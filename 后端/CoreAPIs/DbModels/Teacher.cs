using System;
using System.Collections.Generic;

#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class Teacher
    {
        public int TeacherId { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string TeacherName { get; set; }
        public int IsQuit { get; set; }
        public string Salt { get; set; }
    }
}
