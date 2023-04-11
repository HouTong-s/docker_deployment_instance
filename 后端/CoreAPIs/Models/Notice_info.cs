using CoreAPIs.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class Notice_info
    {
        public List<single_notice> notices { get; set; } = new List<single_notice>();
        public int total { get; set; }
    }
    public class single_notice
    {
        public int notice_id { get; set; }
        public DateTime? Time { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
