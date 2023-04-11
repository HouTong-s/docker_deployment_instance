using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class TokenVerificationResult
    {
        public int id { get; set; } = 0;
        public string Role { get; set; } = "bad user";
    }
}
