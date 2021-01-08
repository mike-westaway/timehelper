/*
 The MIT License (MIT)

Copyright (c) 2018 Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using TimeHelper.Data;
using TimeHelper.Data.Models;
using TimeHelper.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TimeHelper.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarController : Controller
    {
        readonly ITokenAcquisition tokenAcquisition;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TimeHelperDataContext db;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        public CalendarController(IHttpContextAccessor contextAccessor, ITokenAcquisition tokenAcquisition, TimeHelperDataContext db, ILogger<CalendarController> logger, IConfiguration config)
        {
            _contextAccessor = contextAccessor;
            this.tokenAcquisition = tokenAcquisition;
            this.db = db;
            this.logger = logger;
            this.config = config;

        }
        
        // GET: api/Calendar
        /// <summary>
        /// Get a very lightweight list of calendar items for the authenticated user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Calendar/simple
        ///     {
        ///     }
        /// <param name="skip">optional parameter.  how many events to skip when server side paging</param>
        /// <param name="take">optional parameter.  how many events to return when server side paging</param>
        /// <returns>A json list of calendar events</returns>
        /// <response code="200">Success</response>
        /// <response code="204">No event content was found</response> 
        /// </remarks>        
        [HttpGet]
        [Route("simple")]
        [AuthorizeForScopes(Scopes = new[] { "https://graph.microsoft.com/Calendars.Read" })]
        public async Task<ActionResult> CalendarSimple(int? skip, int? take)
        {
            
            dynamic Events = new ExpandoObject();
            if (skip == null) skip = 0;
            if (take == null) take = 10;
 
            var scopes = new[] {@"https://graph.microsoft.com/Calendars.Read"};
            HttpClient httpClient = new HttpClient();
            var graphUrl = $"https://graph.microsoft.com/v1.0/me/events?$select=iCalUId, subject,start,end,location,categories,sensitivity,showas,responsestatus&top={take}&skip={skip}";
            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync($"{graphUrl}");
            string responsePayload = string.Empty;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Events = new ExpandoObject();
                Events.events = new List<ExpandoObject>();
                var jsonContent = JObject.Parse(responseContent);
                foreach (var item in jsonContent["value"])
                {
                    DateTime start = DateTime.Parse(item["start"]["dateTime"].ToString());
                    DateTime end = DateTime.Parse(item["end"]["dateTime"].ToString());
                    dynamic Event = new ExpandoObject();
                    Event.id = item["id"].ToString();
                    Event.iCalUId = item["iCalUId"].ToString();
                    Event.subject = item["subject"].ToString();
                    Event.sensitivity = item["sensitivity"].ToString();
                    Event.showAs = item["showAs"].ToString();
                    Event.startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
                    Event.endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
                    var categories = new List<string>();
                    foreach (var category in item["categories"])
                    {
                        categories.Add(category.ToString());
                    }
                    Event.categories = categories;

                    Event.durationInMinutes = end.Subtract(start).TotalMinutes.ToString();
                    Event.project = "DummyProject";
                    Events.events.Add(Event);
                }
                Events.take = take;
                Events.skip = skip;
                Events.nextSkip = take + skip;
                string temp = JsonConvert.SerializeObject(Events);
                return Ok(Events);
            }
            else 
            {
                var userEmail = User.Claims.Where(c => c.Type == ClaimTypes.Upn)
                  .Select(c => c.Value).SingleOrDefault().ToLower();
                string dummyResponse = TryGenerateDummyResponse(userEmail, "dummy");
                var dummyData = JsonConvert.DeserializeObject<Rootobject>(dummyResponse);
                return Ok(dummyData);
            }
            
        }



        private string TryGenerateDummyResponse(string userEmail, string discriminator)
        {
            var blobData = string.Empty;
            try
            {
                
                string url = $"{config["DummyDataBlobContainer"]}{userEmail}.{discriminator}.json";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    blobData = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw new Exception("No Calendar Entries found for user and dummy user data does not exist.  See inner exception for details.", e);
            }
            return blobData;

            
        }

        // GET: api/Calendar
        [HttpGet]
        //[AuthorizeForScopes(Scopes = new[] { "Calendars.Read" })]
        public async Task<ActionResult<CalendarEventResponse>> Get()
        {
            var scopes = new[] { @"https://graph.microsoft.com/Calendars.Read" };
            try
            {
                HttpClient httpClient = new HttpClient();
                var graphUrl = @"https://graph.microsoft.com/v1.0/me/events?$select=subject,bodyPreview,organizer,attendees,start,end,location,categories,sensitivity,showas,responsestatus";

                var accessToken =
                    await tokenAcquisition.GetAccessTokenForUserAsync(scopes);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync($"{graphUrl}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var calendarEventResponse = JsonConvert.DeserializeObject<CalendarEventResponse>(content);
                    if (calendarEventResponse.value.Count() == 0) return NoContent();
                    return calendarEventResponse;
                }
                return BadRequest($"Http Status Code returned from Graph Api was {response.StatusCode}");
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                await tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeaderAsync(scopes, ex.MsalUiRequiredException);
                return null;
            }
            catch (MsalUiRequiredException ex)
            {
                await tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeaderAsync(scopes, ex);
                return null;
            }
        }

        [HttpGet]
        [Route("relevant")]
        [AuthorizeForScopes(Scopes = new[] { "https://graph.microsoft.com/Calendars.Read" })]
        public async Task<ActionResult> relevant(int? skip, int? take)
        {
            logger.LogInformation($"Request received in relevant controller");
            var userEmail = User.Claims.Where(c => c.Type == ClaimTypes.Upn).Select(c => c.Value).SingleOrDefault().ToLower();
            logger.LogDebug($"Request is on behalf of user {userEmail}");
            dynamic Events = new ExpandoObject();
            if (skip == null) skip = 0;
            if (take == null) take = 10;

            var scopes = new[] { @"https://graph.microsoft.com/Calendars.Read" };
            HttpClient httpClient = new HttpClient();


            var graphUrl = $"https://graph.microsoft.com/v1.0/me/events?$select=iCalUId, subject,start,end,location,categories,sensitivity,showas,responsestatus&$filter=start/dateTime ge '2020-12-14T00:00' and end/dateTime le '2020-12-18T23:59'  ";

           // var graphUrl = $"https://graph.microsoft.com/v1.0/me/events?$select=iCalUId, subject,start,end,location,categories,sensitivity,showas,responsestatus&top={take}&skip={skip}";
            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync($"{graphUrl}");

            string responseContent = string.Empty;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                logger.LogDebug($"Calendar events were retrieved using graph api for user {userEmail}");
                responseContent = await response.Content.ReadAsStringAsync();
            }
            else 
            {
                logger.LogDebug($"Calendar events are using dummy data for user {userEmail}");
                responseContent = TryGenerateDummyResponse(userEmail, "relevant");
            }

            Events = new ExpandoObject();
            Events.events = new List<ExpandoObject>();
            var jsonContent = JObject.Parse(responseContent);
            foreach (var item in jsonContent["value"])
            {
               
                DateTime start = DateTime.Parse(item["start"]["dateTime"].ToString());
                DateTime end = DateTime.Parse(item["end"]["dateTime"].ToString());
                dynamic Event = new ExpandoObject();
                Event.id = item["id"].ToString();
                
                Event.iCalUId = item["iCalUId"].ToString();
                logger.LogDebug($"Processing Event iCalUid {Event.iCalUId}");
                Event.subject = item["subject"].ToString();
                logger.LogDebug($"Subject: {Event.subject}");
                Event.sensitivity = item["sensitivity"].ToString();
                Event.showAs = item["showAs"].ToString();
                Event.startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
                Event.endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
                var categories = new List<string>();
                foreach (var category in item["categories"])
                {
                    categories.Add(category.ToString());
                    logger.LogDebug($"Category: {category.ToString()}");
                }
                Event.categories = categories;

                Event.durationInMinutes = end.Subtract(start).TotalMinutes.ToString();
                Event.project = "";
                Event.projectId = null;
                var projectId = await GetProjectAsync(userEmail, item);
                if (projectId != null)
                {
                    var project = await db.Project.FindAsync(projectId);
                    if (project != null)
                    {
                        logger.LogDebug($"Event iCalUid {Event.iCalUId} was associated to {project.Name}");
                        Event.project = project.Name;
                        Event.projectId = project.ProjectId;
                    }
                }
                Events.events.Add(Event);
            }
            Events.take = take;
            Events.skip = skip;
            Events.nextSkip = take + skip;

            string temp = JsonConvert.SerializeObject(Events);
            return Ok(Events);

        }

        private async Task<int?> GetProjectAsync(string userEmail, JToken item)
        {
            logger.LogDebug($"Should this event be associated to a project ofr user {userEmail}?");
            var userAssociations =  db.Association.Where(a => a.UserId == userEmail);
            var temp = item.ToString();
            foreach (var association in userAssociations)
            {
                logger.LogDebug($"AssociationId={association.AssociationId}, type={association.Type}");
                switch (association.Type)
                {
                    case AssociationType.Subject:
                        logger.LogDebug($"checking subject {item["subject"].ToString()} against criteria {association.Criteria} ");
                        if (item["subject"].ToString().Contains(association.Criteria, StringComparison.InvariantCultureIgnoreCase))
                            return association.ProjectId;
                        break;
                    case AssociationType.Category:
                        logger.LogDebug($"checking categories against criteria {association.Criteria} ");
                        foreach (var category in item["categories"])
                        {
                            logger.LogDebug($"category is {category} ");
                            if (association.Criteria.Equals(category.ToString(), StringComparison.InvariantCultureIgnoreCase)) 
                            {
                                return association.ProjectId;
                            } 
                                
                        }
                        break;
                    default:
                        logger.LogDebug($"no projects identified (unknown type)");
                        return null;
                        
                }
            }
            logger.LogDebug($"no projects identified");
            return null;
        }
    }
}