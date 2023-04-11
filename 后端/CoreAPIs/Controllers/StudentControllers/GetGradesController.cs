using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.StudentControllers
{
    [Route("api/students/[controller]")]
    [ApiController]
    public class GetGradesController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetGradesController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/students/GetGrades
        [HttpGet]
        /*
         *@TODO:获取所有的成绩
         *@param  null
         *@return 
         *成功返回Grades_info对象
         *失败返回状态码
         */
        public async Task<ActionResult<Grades_info>> GetGrades()
        {
            int student_id = Convert.ToInt32(Request.Headers["ids"]);
            var lessons =await 
                        (from util in
                          (from enroll in
                                (from enrollment in _context.Enrollments
                                 where enrollment.StudentId == student_id && enrollment.GradeStatus !="未出成绩"
                                 select enrollment)
                           join schedule in _context.Schedules
                            on enroll.ScheduleId equals schedule.ScheduleId
                           select new
                           {
                               enroll = enroll,
                               schedule = schedule
                           })
                           join lesson in _context.Lessons
                           on util.schedule.LessonId equals lesson.LessonId
                           select new SingleGrade_info
                           {
                               lesson = lesson,
                               year = (int)util.schedule.Year,
                               half = (int)util.schedule.Half,
                               time = (DateTime)util.enroll.InputTime,
                               Score = (int)util.enroll.Score,
                               GradePoint = (int)util.enroll.GradePoint,
                               status = util.enroll.GradeStatus,
                               is_pass = util.enroll.Score >= 60,
                               current_id = lesson.LessonId

                           }).ToListAsync();
            if(lessons.Count==0)
            {
                return NotFound();
            }
            decimal total_grade_points = 0;
            decimal total_credits = 0;
            foreach(var i in lessons)
            {
                total_credits += (decimal)i.lesson.Credit;
                total_grade_points += (decimal) i.lesson.Credit * (decimal)i.GradePoint;
            }
            decimal avg = total_grade_points / total_credits;
            
            var lesson_ids = lessons.Select(s=>s.lesson.LessonId).Distinct();
            decimal pass_credits = 0;
            decimal failed_credits = 0;
            int falied_num = 0;
            //以下主要处理的是重修课和新修课以及课程变更之后的成绩统计算法
            Dictionary<int, int> repeated = new Dictionary<int, int>();
            foreach (int i in lesson_ids)
            {
                var past_lesson = await(from lesson in _context.Lessons
                                        where lesson.PreqId != null 
                                           && lesson.LessonId == i
                                        select lesson.PreqId).FirstOrDefaultAsync();
                while(past_lesson != null)
                {
                    if(repeated.ContainsKey((int)past_lesson))
                    {
                          repeated.Remove((int)past_lesson);
                    }
                    repeated.Add((int)past_lesson,i );
                    past_lesson = await (from lesson in _context.Lessons
                                             where lesson.PreqId != null
                                                && lesson.LessonId == past_lesson
                                         select lesson.PreqId).FirstOrDefaultAsync();
                    //遍历循环，查找这个课程对应的所有的以前课程，并记录在字典中
                }
            }
            foreach (int i in lesson_ids)
                //让字典最终都记录课程的最终id号
            {
                if(!repeated.ContainsKey(i))
                {
                    continue;
                }
                int re = i;
                while (repeated.ContainsKey(re))
                {
                    re = repeated[re];
                }
                repeated[i] = re;
            }           
            foreach (int i in lesson_ids)
            {
                if(!repeated.ContainsKey(i) && !repeated.ContainsValue(i))
                {
                    //没有课程变更的情况
                    var a = lessons.Where(s => s.lesson.LessonId == i);
                    var b = a.Where(s => s.is_pass).FirstOrDefault();
                    if (b != null)
                    {
                        pass_credits += (decimal)b.lesson.Credit;
                    }
                    else
                    {
                        falied_num += 1;
                        failed_credits += (decimal)a.FirstOrDefault().lesson.Credit;
                    }
                }
                else if(!repeated.ContainsKey(i) && repeated.ContainsValue(i))
                    //这个时候是最老版本的lesson对应的那一组lesson(避免重复)
                {
                    List<int> temp = new List<int>();
                    temp.Add(i);
                    foreach(KeyValuePair<int,int> j in repeated)
                    {
                        if (j.Value == i)
                            temp.Add(j.Key);
                    }
                    var a = lessons.Where(s => temp.Contains(s.lesson.LessonId));
                    var b = a.Where(s => s.is_pass).FirstOrDefault();
                    if (b != null)
                    {
                        pass_credits += (decimal)b.lesson.Credit;
                    }
                    else
                    {
                        falied_num += 1;
                        failed_credits += (decimal)a.FirstOrDefault().lesson.Credit;
                    }
                }
            }
            foreach(var i in lessons)
            {
                if(repeated.ContainsKey((int)i.current_id))
                    i.current_id = repeated[(int)i.current_id];
            }
            var results = new Grades_info
            {
                grades = lessons,
                avg_grade_point = avg,
                taked_credits = pass_credits,
                falied_credits = failed_credits,
                failed_num = falied_num
            };
            if (lessons.Count > 0)
                return results;
            else
                return NotFound();
        }
    }
}

