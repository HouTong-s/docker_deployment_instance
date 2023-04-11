using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class Schedule_student_info
    {
        public List<Student_basic_info> Students { get; set; } = new List<Student_basic_info>();
        public int ScheduleId { get; set; }
        public string LessonName { get; set; }
        public string Place { get; set; }
        public int? CurrentNum { get; set; }
        public int? MaxNum { get; set; }
        public string Campus { get; set; }
        public int? Year { get; set; }
        public int? Half { get; set; }
        public int? IsOver { get; set; }
        public int? CanImportGrade { get; set; }
    }
    public class Student_basic_info
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Department { get; set; }
        public int? score { get; set; }
        public string? GradeStatus { get; set; }
    }
}
