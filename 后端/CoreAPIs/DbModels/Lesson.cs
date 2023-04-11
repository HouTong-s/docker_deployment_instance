using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class Lesson
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public string Type { get; set; }
        public decimal Credit { get; set; }
        public int? PreqId { get; set; }
        public string Note { get; set; }
        public string NeedDepart { get; set; }
        public string Identity { get; set; }
    }
}
