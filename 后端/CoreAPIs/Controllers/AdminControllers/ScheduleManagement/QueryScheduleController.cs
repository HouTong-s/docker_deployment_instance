using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.ScheduleManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class QueryScheduleController : ControllerBase
    {
        private readonly schoolContext _context;

        public QueryScheduleController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/QuerySchedule
        [HttpGet]
      /*
       *@TODO:查询id对应的排课信息
       *@param {int} schedule_id 排课id
       *
       *@return 
       *成功返回schedule_times_info对象
       *失败返回状态码
       *
       */
        public async Task<ActionResult<schedule_times_info>> GetSchedules([FromQuery]int schedule_id)
        {
            var schedule = await (from _schedule in _context.Schedules
                                where _schedule.ScheduleId == schedule_id
                                  select _schedule).FirstOrDefaultAsync();
            var times = await (from time in _context.ScheduleTimes
                               where time.ScheduleId == schedule_id
                               select time).ToListAsync();
            var _lesson = await (from lessson in _context.Lessons
                                 where lessson.LessonId == schedule.LessonId
                                 select lessson).FirstOrDefaultAsync();
            var teacher_name = await (from teacher in _context.Teachers
                                      where teacher.TeacherId == schedule.TeacherId
                                      select teacher.TeacherName).FirstOrDefaultAsync();
            if (schedule == null)
            {
                return NotFound();
            }
            else
            {
                return new schedule_times_info
                {
                    schedule = schedule,
                    lesson_name = _lesson.LessonName,
                    credit = _lesson.Credit,
                    teacher_name = teacher_name,
                    times = times
                };
            }
        }
    }
}
