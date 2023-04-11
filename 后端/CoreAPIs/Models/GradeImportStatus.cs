using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class GradeImportStatus
    {
        public int status { get; set; }
        public DateTime beginTime { get; set; }
        public DateTime endTime { get; set; }
    }
}
