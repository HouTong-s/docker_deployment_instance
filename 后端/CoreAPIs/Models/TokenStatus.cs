using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class TokenStatus
    {
        public string token { get; set; }
        public int status { get; set; }
        public TokenStatus(string v1, int v2)
        {
            this.token = v1;
            this.status = v2;
        }

       
    }
}
