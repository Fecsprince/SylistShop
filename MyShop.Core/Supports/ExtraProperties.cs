using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShop.Core.Supports
{


    public class WorkingDays
    {
        public string Name { get; set; }

    }

    public class Time
    {
        public string TimeLabel { get; set; }
    }

    public class ExtraProperties
    {

        public IEnumerable<WorkingDays> GetWorkingDays()
        {
            List<WorkingDays> workingDays = new List<WorkingDays>()
            {
                new WorkingDays(){Name = "MON-FRI"},
                new WorkingDays(){Name = "MON-SUN"},
                new WorkingDays(){Name = "MON-SAT"}
            };
            return workingDays;
        }

        public IEnumerable<Time> TimeList()
        {
            return new List<Time>()
            {
                new Time() { TimeLabel = "8:00"},
                new Time() { TimeLabel = "8:30"},
                new Time() { TimeLabel = "9:00"},
                new Time() { TimeLabel = "9:30"},
                new Time() { TimeLabel = "10:00"},
                new Time() { TimeLabel = "10:30"},
                new Time() { TimeLabel = "11:00"},
                new Time() { TimeLabel = "11:30"},
                new Time() { TimeLabel = "12:00"},
                new Time() { TimeLabel = "12:30"},
                new Time() { TimeLabel = "13:00"},
                new Time() { TimeLabel = "13:30"},
                new Time() { TimeLabel = "14:00"},
                new Time() { TimeLabel = "14:30"},
                new Time() { TimeLabel = "15:00"},
                new Time() { TimeLabel = "15:30"},
                new Time() { TimeLabel = "16:00"},
                new Time() { TimeLabel = "16:30"},
                new Time() { TimeLabel = "17:00"},
                new Time() { TimeLabel = "18:30"},
                new Time() { TimeLabel = "19:00"},
                new Time() { TimeLabel = "19:30"},
                new Time() { TimeLabel = "20:00"},
                new Time() { TimeLabel = "20:30"},
                new Time() { TimeLabel = "21:00"},
                new Time() { TimeLabel = "21:30"}
            };
        }

    }
}