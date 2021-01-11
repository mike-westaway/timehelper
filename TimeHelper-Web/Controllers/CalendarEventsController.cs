using TimeHelper.Web.Models;
using TimeHelper.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;


namespace TimeHelper.Web.Controllers
{
    [Authorize]
    public class CalendarEventsController : Controller
    {
        private ICalendarService calendarService;

        public CalendarEventsController(ICalendarService calendarService)
        {
            this.calendarService = calendarService;
        }

        // GET: TodoList
        [AuthorizeForScopes(ScopeKeySection = "Api:TimeHelperApiScope")]
        public async Task<ActionResult> Index()
        {
         
            var events = await calendarService.GetAsync();
            return View(events.value);
        }



        [AuthorizeForScopes(ScopeKeySection = "Api:TimeHelperApiScope")]
        [Route("api/[controller]/simple")]
        public async Task<ActionResult> Simple(int? skip, int? take)
        {
            var response = await calendarService.SimpleAsync(skip, take);
            return View("Simple", response);
        }

        [AuthorizeForScopes(ScopeKeySection = "Api:TimeHelperApiScope")]
        [Route("api/[controller]/relevant")]
        public async Task<ActionResult> Relevant(int? skip, int? take)
        {
            var response = await calendarService.RelevantAsync(skip, take);
            return View("Relevant", response);
        }
    }
}