using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.DbModels;
namespace CoreAPIs.Models
{
    public class SingleGrade_info 
    {
        public Lesson lesson { get; set; }
        public DateTime time { get; set; }
        public int year { get; set; }
        public int half { get; set; }
        public int Score { get; set; }
        public int GradePoint { get; set; }
        public string status { get; set; }
        public bool is_pass { get; set; }
        public int? current_id { get; set; } = null;
    }
    public class Grades_info
    {
        public List<SingleGrade_info> grades {set;get;}
        public decimal avg_grade_point { set; get; }
        public decimal taked_credits { set; get; }
        public decimal falied_credits { set; get; }
        public int failed_num { set; get; }
    }
}
