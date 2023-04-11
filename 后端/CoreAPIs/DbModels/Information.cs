using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class Information
    {
        public int Year { get; set; }
        public int Half { get; set; }
        public int SelectStatus { get; set; }
        public DateTime? SelectBeginTime { get; set; }
        public DateTime? SelectEndTime { get; set; }
        public DateTime SemesterBeginTime { get; set; }
        public int CanImportGrade { get; set; }
        public DateTime? GradeBeginTime { get; set; }
        public DateTime? GradeEndTime { get; set; }
        public string Id { get; set; }
    }
}
