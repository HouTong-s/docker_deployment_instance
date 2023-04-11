using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Department { get; set; }
        public int OriginInyear { get; set; }
        public int InYear { get; set; }
        public string Password { get; set; }
        public string Identity { get; set; }
        public int IsGraduate { get; set; }
        public string Salt { get; set; }
    }
}
