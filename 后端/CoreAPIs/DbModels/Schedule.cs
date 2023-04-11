using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int LessonId { get; set; }
        public int TeacherId { get; set; }
        public int Year { get; set; }
        public int Half { get; set; }
        public int IsOver { get; set; }
        public int BeginWeek { get; set; }
        public int EndWeek { get; set; }
        public string Place { get; set; }
        public int CurrentNum { get; set; }
        public int MaxNum { get; set; }
        public string Note { get; set; }
        public string TeachingMaterial { get; set; }
        public int CanRetake { get; set; }
        public string Campus { get; set; }
    }
}
