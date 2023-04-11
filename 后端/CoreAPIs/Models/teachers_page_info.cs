using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    using DbModels;
    public class teachers_page_info
    {
        
        public List<Teacher> teachers { get; set; } = new List<Teacher>();
        public int total { get; set; }
    }
}
