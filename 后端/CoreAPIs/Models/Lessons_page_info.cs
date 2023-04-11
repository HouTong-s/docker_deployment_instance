using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.DbModels;

namespace CoreAPIs.Models
{
    public class Lessons_page_info
    {
        public List<Lesson_requirement_info> lessons { get; set; } = new List<Lesson_requirement_info>();
        public int total { get; set; }
    }
    public class Lesson_requirement_info
    {
        public Lesson lesson { get; set; }
        public List<LessonRequirement> requirements { get; set; } = new List<LessonRequirement>();
    }
}
