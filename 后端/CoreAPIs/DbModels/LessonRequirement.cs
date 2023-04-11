using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class LessonRequirement
    {
        public int LessonId { get; set; }
        public int InYear { get; set; }
        public int MinGrade { get; set; }
        public int MaxGrade { get; set; }
    }
}
