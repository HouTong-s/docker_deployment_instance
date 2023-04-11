using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.DbModels;
namespace CoreAPIs.Models
{
    public class Lessons_info
    {
        public List<Lesson> lessons { get; set; }
        public DateTime begin_time { get; set; }
        public DateTime end_time { get; set; }
        public string select_type { get; set; }
    }
}
