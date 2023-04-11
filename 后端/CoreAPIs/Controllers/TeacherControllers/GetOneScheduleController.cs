using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class GetOneScheduleController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetOneScheduleController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/teachers/GetOneSchedule
        [HttpGet]
        /*
         *@TODO:获取老师的某节排课的具体信息
         *@param {int} schedule_id 排课号
         *@return 
         *成功返回Schedule_student_info对象
         *失败返回状态码
         */
        public async Task<ActionResult<Schedule_student_info>> GetOneSchedule([FromQuery]int schedule_id)
        {
            
            var info = new Schedule_student_info();
            
            int teacher_id = Convert.ToInt32(Request.Headers["ids"]);
            var _schedule = await (from schedule in _context.Schedules
                           where schedule.ScheduleId == schedule_id
                           select schedule).FirstOrDefaultAsync();
            if (_schedule != null)
            {
                var _information = await (from information in _context.Information
                                          select information).FirstOrDefaultAsync();
                DateTime dt = DateTime.Now;
                if(_information.CanImportGrade == 0 || _information.GradeBeginTime > dt || _information.GradeEndTime < dt)
                    info.CanImportGrade = 0;
                else
                    info.CanImportGrade = 1;
                var results = await (from en in
                                       (from sch in
                                            (from schedule in _context.Schedules
                                             where schedule.ScheduleId == schedule_id
                                             select schedule)
                                        join enrollment in _context.Enrollments
                                        on sch.ScheduleId equals enrollment.ScheduleId
                                        select enrollment)
                                     orderby en.StudentId
                                     join student in _context.Students
                                     on en.StudentId equals student.StudentId
                                     select new Student_basic_info
                                     {
                                         StudentId = en.StudentId,
                                         score = en.Score,
                                         GradeStatus = en.GradeStatus,
                                         StudentName = student.StudentName,
                                         Department = student.Department
                                     }
                                   ).ToListAsync();
                info.Students = results;
                info.IsOver = _schedule.IsOver;
                info.Year = _schedule.Year;
                info.Half = _schedule.Half;
                info.CurrentNum = _schedule.CurrentNum;
                info.MaxNum = _schedule.MaxNum;
                info.Place = _schedule.Place;
                info.ScheduleId = schedule_id;
                info.Campus = _schedule.Campus;

                var _lesson = await (from lesson in _context.Lessons
                                     where lesson.LessonId == _schedule.LessonId
                                     select lesson).FirstOrDefaultAsync();
                info.LessonName = _lesson.LessonName;

                return info;
            }
            else
            {
                return NotFound();
            }
        }

    }
}
