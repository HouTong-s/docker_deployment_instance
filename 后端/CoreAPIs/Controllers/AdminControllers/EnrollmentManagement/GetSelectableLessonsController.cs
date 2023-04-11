using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.EnrollmentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class GetSelectableLessonsController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetSelectableLessonsController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/GetSelectableLessons
        [HttpGet]
        /*
        *@TODO:获取某学生的可选课程
        *@param {int} select_status 选课状态（1或2）
        *@param {int} student_id 学生id
        *
        *@return 
        *成功返回Lesson对象数组
        *失败返回状态码400
        */
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons([FromQuery]int student_id, [FromQuery] int select_status)
        {
            //int student_id = Convert.ToInt32(Request.Headers["ids"]);
            var information = await _context.Information.FirstOrDefaultAsync();
            var begin_time = information.SelectBeginTime;//选课的起始时间，作为附加的信息
            var end_time = information.SelectEndTime;
            DateTime dt = DateTime.Now;
            if (begin_time == null || end_time == null || !(begin_time <= dt && dt <= end_time))
            {
                return BadRequest();
            }
            var _student = await _context.Students.Where(s => s.StudentId == student_id).Select(s => s).FirstOrDefaultAsync();
            var in_year = _student.InYear;
            int grade = (int)(information.Year - _student.InYear + information.Half);
            var major = _student.Department;
            var identity = _student.Identity;
            if (select_status == 1)//新修
            {
                //以下大前提是这学期有相关排课的才能选进课程(如果没有排课的就舍弃)
                var lesson_IDs = await (from i in
                                       (from requirement in _context.LessonRequirements
                                        where requirement.InYear == in_year
                                        && requirement.MinGrade <= grade
                                        && grade <= requirement.MaxGrade
                                        select requirement.LessonId)
                                        join schedule in _context.Schedules
                                        on i equals schedule.LessonId
                                        where schedule.Year == information.Year
                                           && schedule.Half == information.Half
                                        select i
                                         ).ToListAsync();//找到符合选课要求的课程id
                var hashset = lesson_IDs.Distinct();
                //转换成集合，去除重复元素(distinct也能去除重复)
                var lessons = new List<Lesson>();
                foreach (int lessonid in hashset)
                {
                    int lesson_id = lessonid;
                    //下面就是一个学生哪几门课已经修过了(isover=1)，只能等着重修了，这是新修课就选不上了，或者is_over==0，正在修的也不能选了
                    var selected_lessons_id = await (from schedule in _context.Schedules
                                                     join enroll in
                                                     (from enrollment in _context.Enrollments
                                                      where enrollment.StudentId == student_id
                                                      select enrollment)
                                                     on schedule.ScheduleId equals enroll.ScheduleId
                                                     select schedule.LessonId).ToListAsync();
                    var _lesson = await (from lesson in _context.Lessons
                                         where lesson_id == lesson.LessonId
                                         && !selected_lessons_id.Contains(lesson_id)
                                         && (major == lesson.NeedDepart || lesson.NeedDepart == "all")
                                         && lesson.Identity == identity
                                         //还要对比课程所需的专业，一般来说一个课程的所需专业是一定的，所以放在lesson表里面作为一个固定的属性
                                         select lesson).FirstOrDefaultAsync();
                    if (_lesson != null)
                        lessons.Add(_lesson);
                }
                return lessons;
            }
            else if (select_status == 2)//重修
            {
                //以下大前提是修过的才能选进课程(如果没有排课的就舍弃)
                var selected_lessons_id = await (from i in
                                               (from schedule in _context.Schedules
                                                join enroll in
                                                (from enrollment in _context.Enrollments
                                                 where enrollment.StudentId == student_id
                                                 select enrollment)
                                                on schedule.ScheduleId equals enroll.ScheduleId
                                                where schedule.IsOver == 1
                                                select schedule.LessonId)
                                                 join schedule in _context.Schedules
                                                 on i equals schedule.LessonId
                                                 where schedule.Year == information.Year
                                                  && schedule.Half == information.Half
                                                 select i
                                               ).ToListAsync();//isover=1意味着已经结课的课程，重修课就得在这其中选择(同时本学期也要有排课才行)
                var hashset = selected_lessons_id.ToHashSet();
                var excluded_lessons_id = await (from schedule in _context.Schedules
                                                 join enroll in
                                                 (from enrollment in _context.Enrollments
                                                  where enrollment.StudentId == student_id
                                                  select enrollment)
                                                 on schedule.ScheduleId equals enroll.ScheduleId
                                                 where schedule.IsOver == 0
                                                 select schedule.LessonId).ToListAsync();//这学期刚选的课程(isover = 0)肯定不能再重修了，要除外才行
                var lessons = new List<Lesson>();
                foreach (int lesson_id in hashset)
                {
                    var _lesson = await (from lesson in _context.Lessons
                                         where lesson_id == lesson.LessonId
                                            && !excluded_lessons_id.Contains(lesson_id)
                                            && (major == lesson.NeedDepart || lesson.NeedDepart == "all")
                                            && lesson.Identity == identity
                                         //还要对比课程所需的专业，一般来说一个课程的所需专业是一定的，所以放在lesson表里面作为一个固定的属性
                                         select lesson).FirstOrDefaultAsync();
                    if (_lesson != null)
                        lessons.Add(_lesson);
                }
                return lessons;
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
