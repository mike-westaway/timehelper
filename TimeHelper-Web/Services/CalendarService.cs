// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using TimeHelper.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TimeHelper.Web.Services
{
    public static class CalendarServiceExtensions
    {
        public static void AddCalendarService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<ICalendarService, CalendarService>();
        }
    }

    /// <summary></summary>

    public class CalendarService : ICalendarService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _TimeHelperApiScope = string.Empty;
        private readonly string _TimeHelperApiBaseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public CalendarService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _TimeHelperApiScope = configuration["Api:TimeHelper-Api-Scope"];
            _TimeHelperApiBaseAddress = configuration["Api:TimeHelper-Api-BaseAddress"];
        }

        public async Task<CalendarEventResponse> GetAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync($"{ _TimeHelperApiBaseAddress}/api/calendar");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var calendarEvents = JsonConvert.DeserializeObject<CalendarEventResponse>(content);
                return calendarEvents;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }


        public async Task<string> SimpleAsync(int? skip, int? take)
        {
            string url = $"{ _TimeHelperApiBaseAddress}/api/calendar/simple";
            if ((skip != null) || (take != null)) 
            {
                if (skip != null)
                {
                    url += $"?skip={skip}";
                    if (take != null) url += $"&take={take}";
                }
                else
                {
                    if (take != null) url += $"?take={take}";
                    
                }
            }
            

            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
             var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _TimeHelperApiScope });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> RelevantAsync(int? skip, int? take)
        {
            string url = $"{ _TimeHelperApiBaseAddress}/api/calendar/relevant";
            if ((skip != null) || (take != null))
            {
                if (skip != null)
                {
                    url += $"?skip={skip}";
                    if (take != null) url += $"&take={take}";
                }
                else
                {
                    if (take != null) url += $"?take={take}";

                }
            }

            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
    }
}