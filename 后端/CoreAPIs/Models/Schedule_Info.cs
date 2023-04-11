using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.DbModels;
namespace CoreAPIs.Models
{
    public class Schedule_Info
    {
        public int ScheduleId { get; set; }
        public int LessonId { get; set; }
        public int TeacherId { get; set; }
        public int? BeginWeek { get; set; }
        public int? EndWeek { get; set; }
        public string Place { get; set; }
        public int? CurrentNum { get; set; }
        public int? MaxNum { get; set; }
        public string Note { get; set; }
        public int selectstatus { get; set; }
        public string Type { get; set; }
        public string TeachingMaterial { get; set; }
        public string Campus { get; set; }
        public List<ScheduleTime> times { get; set; }
        public string teacher_name { get; set; }
        public string lesson_name { get; set; }
        public decimal credit { get; set; }
    }
    public class Teacher_Schedules
    {
        public List<Schedule_Info> schedules { get; set; } = new List<Schedule_Info>();
        public int total { get; set; }
    }

}