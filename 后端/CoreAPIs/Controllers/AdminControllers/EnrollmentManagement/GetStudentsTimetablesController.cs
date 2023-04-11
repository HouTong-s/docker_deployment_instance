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
    public class GetStudentsTimetablesController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetStudentsTimetablesController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/GetStudentsTimetables
        [HttpGet]
        /*
        *@TODO:获取某学生的课表
        *@param {int} student_id 学生id
        *@param {int} year 年份
        *@param {int} half 半年
        *
        *@return 
        *成功返回Schedule_Info对象数组
        *失败返回状态码404
        */
        public async Task<ActionResult<IEnumerable<Schedule_Info>>> GetEnrollments([FromQuery] int student_id, [FromQuery] int year, [FromQuery] int half)
        {
            var schedules = await
                             (from un in
                               (from enroll in
                                   (from enrollment in _context.Enrollments
                                    where enrollment.StudentId == student_id
                                    select enrollment)
                                join schedule in _context.Schedules
                                on enroll.ScheduleId equals schedule.ScheduleId
                                where schedule.Year == year && schedule.Half == half
                                select new
                                {
                                    enroll = enroll,
                                    schedule = schedule
                                })
                              join lesson in _context.Lessons
                              on un.schedule.LessonId equals lesson.LessonId
                              select new Schedule_Info
                              {
                                  ScheduleId = un.schedule.ScheduleId,
                                  LessonId = un.schedule.LessonId,
                                  TeacherId = un.schedule.TeacherId,
                                  BeginWeek = un.schedule.BeginWeek,
                                  EndWeek = un.schedule.EndWeek,
                                  Place = un.schedule.Place,
                                  CurrentNum = un.schedule.CurrentNum,
                                  MaxNum = un.schedule.MaxNum,
                                  Note = un.schedule.Note,
                                  TeachingMaterial = un.schedule.TeachingMaterial,
                                  Campus = un.schedule.Campus,
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
                var t_name = (from teacher in _context.Teachers
                              where teacher.TeacherId == schedule.TeacherId
                              select teacher.TeacherName).FirstOrDefault();
                var times = (from time in _context.ScheduleTimes
                             where time.ScheduleId == schedule.ScheduleId
                             select time).ToList();
                schedule.teacher_name = t_name;
                schedule.times = times;
            }
            return schedules;
        }
    }
}
