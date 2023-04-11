using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.DbModels;

namespace CoreAPIs.Models
{
    public class Schedules_page_info
    {
        public List<schedule_times_info> schedules { get; set; } = new List<schedule_times_info>();
        public int total { get; set; }
    }
    public class schedule_times_info
    {
        public Schedule schedule { get; set; }
        public string teacher_name { get; set; }
        public string lesson_name { get; set; }
        public decimal credit { get; set; }
        public List<ScheduleTime> times { get; set; } = new List<ScheduleTime>();
    }
}
