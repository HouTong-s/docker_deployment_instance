using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using System.Collections;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.StudentControllers
{
    [Route("api/students/[controller]")]
    [ApiController]
    public class GetSchedulesController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetSchedulesController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/students/GetSchedules/5
        [HttpGet("{lesson_id}")]
        /*
         *@TODO:获取排课（根据当前时间决定是新修还是重修）
	     *@param {int} lesson_id 课程id
	     *@return 
	     *成功返回Schedule_Info对象数组
	     *失败返回状态码
         */
        public async Task<ActionResult<List<Schedule_Info>>> GetSchedules(int lesson_id)
        {

            var information = await _context.Information.FirstOrDefaultAsync();
            if (information.SelectStatus == 1)
            {
                var schedules = await (from schedule in _context.Schedules
                                       where lesson_id == schedule.LessonId
                                       && schedule.Year == information.Year
                                       && schedule.Half == information.Half
                                       //对应的课程和学期必须一致
                                       select schedule).ToListAsync();
                var results = new List<Schedule_Info>();
                foreach (var schedule in schedules)
                {
                    var sche_info = new Schedule_Info();
                    sche_info.ScheduleId = schedule.ScheduleId;
                    sche_info.LessonId = schedule.LessonId;
                    sche_info.TeacherId = schedule.TeacherId;
                    sche_info.BeginWeek = schedule.BeginWeek;
                    sche_info.EndWeek = schedule.EndWeek;
                    sche_info.Place = schedule.Place;
                    sche_info.CurrentNum = schedule.CurrentNum;
                    sche_info.MaxNum = schedule.MaxNum;
                    sche_info.Note = schedule.Note;
                    sche_info.TeachingMaterial = schedule.TeachingMaterial;
                    sche_info.Campus = schedule.Campus;
                    var times = await (from time in _context.ScheduleTimes
                                       where time.ScheduleId == schedule.ScheduleId
                                       select time).ToListAsync();
                    var name = await (from teacher in _context.Teachers
                                      where schedule.TeacherId == teacher.TeacherId
                                      select teacher.TeacherName).FirstOrDefaultAsync();
                    var _lesson = await (from lesson in _context.Lessons
                                         where lesson.LessonId == schedule.LessonId
                                         select lesson).FirstOrDefaultAsync();
                    sche_info.times = times;
                    sche_info.teacher_name = name;
                    sche_info.lesson_name = _lesson.LessonName;
                    sche_info.credit = (decimal)_lesson.Credit;
                    sche_info.Type = _lesson.Type;
                    results.Add(sche_info);
                }
                if (results.Count > 0)
                    return results;
                else
                    return NotFound();
            }
            else if(information.SelectStatus == 2)
            {
                var schedules = await (from schedule in _context.Schedules
                                       where lesson_id == schedule.LessonId
                                       && schedule.Year == information.Year
                                       && schedule.Half == information.Half
                                       && schedule.CanRetake == 1
                                       //对应的课程和学期必须一致
                                       select schedule).ToListAsync();
                var results = new List<Schedule_Info>();
                foreach (var schedule in schedules)
                {
                    var sche_info = new Schedule_Info();
                    sche_info.ScheduleId = schedule.ScheduleId;
                    sche_info.LessonId = schedule.LessonId;
                    sche_info.TeacherId = schedule.TeacherId;
                    sche_info.BeginWeek = schedule.BeginWeek;
                    sche_info.EndWeek = schedule.EndWeek;
                    sche_info.Place = schedule.Place;
                    sche_info.CurrentNum = schedule.CurrentNum;
                    sche_info.MaxNum = schedule.MaxNum;
                    sche_info.Note = schedule.Note;
                    sche_info.TeachingMaterial = schedule.TeachingMaterial;
                    sche_info.Campus = schedule.Campus;
                    var times = await (from time in _context.ScheduleTimes
                                       where time.ScheduleId == schedule.ScheduleId
                                       select time).ToListAsync();
                    var name = await (from teacher in _context.Teachers
                                      where schedule.TeacherId == teacher.TeacherId
                                      select teacher.TeacherName).FirstOrDefaultAsync();
                    var _lesson = await (from lesson in _context.Lessons
                                         where lesson.LessonId == schedule.LessonId
                                         select lesson).FirstOrDefaultAsync();
                    sche_info.times = times;
                    sche_info.teacher_name = name;
                    sche_info.lesson_name = _lesson.LessonName;
                    sche_info.credit = (decimal)_lesson.Credit;
                    results.Add(sche_info);
                }
                if (results.Count > 0)
                    return results;
                else
                    return NotFound();
            }
            else
            {
                return Content("未到选课时间");
            }         
        }
    }
}
