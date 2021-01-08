using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeHelper.Api
{

    public class Rootobject
    {
        public Event[] Events { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public int NextSkip { get; set; }
    }

    public class Event
    {
        public string Id { get; set; }
        public string ICalUId { get; set; }
        public string Subject { get; set; }
        public string Sensitivity { get; set; }
        public string ShowAs { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string[] Categories { get; set; }
        public string DurationInMinutes { get; set; }
        public string Project { get; set; }
    }

}
