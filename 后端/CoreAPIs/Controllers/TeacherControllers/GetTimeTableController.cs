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
    public class GetTimeTableController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetTimeTableController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/teachers/GetTimeTable
        [HttpGet]
        /*
         *@TODO:获取某一学期的课程表（老师的）
         *@param {int} year 年份 
         *@param {int} half 上下半年
         *@return 
         *成功返回Schedule_Info对象数组
         *失败返回状态码
         */
        public async Task<ActionResult<List<Schedule_Info>>> GetTimetable([FromQuery] int year, [FromQuery] int half)
        {
            int teacher_id = Convert.ToInt32(Request.Headers["ids"]);
            var schedules = await (from sche in 
                          (from schedule in _context.Schedules
                           where schedule.Year == year
                              && schedule.Half == half
                              && schedule.TeacherId == teacher_id
                           select schedule)
                           join lesson in _context.Lessons
                           on sche.LessonId equals lesson.LessonId
                           select new Schedule_Info
                           {
                               ScheduleId = sche.ScheduleId,
                               LessonId = sche.LessonId,
                               TeacherId = sche.TeacherId,
                               BeginWeek = sche.BeginWeek,
                               EndWeek = sche.EndWeek,
                               Place = sche.Place,
                               CurrentNum = sche.CurrentNum,
                               MaxNum = sche.MaxNum,
                               Note = sche.Note,
                               TeachingMaterial = sche.TeachingMaterial,
                               Campus = sche.Campus,
                               credit = (decimal)lesson.Credit,
                               Type = lesson.Type,
                               lesson_name = lesson.LessonName
                           }).ToListAsync();
            if (schedules.Count == 0)
            {
                return NotFound();
            }
            foreach (var schedule in schedules)
            {
                var times = (from time in _context.ScheduleTimes
                             where time.ScheduleId == schedule.ScheduleId
                             select time).ToList();
                schedule.times = times;
            }
            return schedules;
        }
    }
}
