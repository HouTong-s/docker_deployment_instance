using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class ScheduleTimes_Info
    {
        public int begin_week { get; set; }
        public int end_week { get; set; }
        public List<SingleTime> times { get; set; }
    }
    public class SingleTime
    {
        public int begin_section { get; set; }
        public int end_section { get; set; }
        public int dayweek { get; set; }
        public int single_or_double { get; set; }
    }
}
