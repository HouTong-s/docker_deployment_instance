using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class ScheduleTime
    {
        public int ScheduleId { get; set; }
        public int BeginSection { get; set; }
        public int EndSection { get; set; }
        public int DayWeek { get; set; }
        public int SingleOrDouble { get; set; }
    }
}
