using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class Enrollment
    {
        public int StudentId { get; set; }
        public int ScheduleId { get; set; }
        public int? Score { get; set; }
        public int? GradePoint { get; set; }
        public string GradeStatus { get; set; }
        public int SelectStatus { get; set; }
        public DateTime? InputTime { get; set; }
    }
}
