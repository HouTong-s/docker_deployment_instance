using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    using DbModels;
    public class students_page_info
    {
        public List<Student> students { get; set; } = new List<Student>();
        public int total { get; set; }
    }
}
