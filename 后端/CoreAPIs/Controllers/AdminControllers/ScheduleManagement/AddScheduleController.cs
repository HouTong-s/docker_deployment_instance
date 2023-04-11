using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.ScheduleManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddScheduleController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddScheduleController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/RegisterSchedule
        [HttpPost]
        /*
        *@TODO:新增一条排课信息
        *@param {int} lesson_id 课程id
        *@param {int} teacher_id 老师id
        *@param {int} year 年份
        *@param {int} half 半年
        *@param {int} begin_week 开始周
        *@param {int} end_week 结束周
        *@param {int} max_num 最多人数
        *@param {int} can_retake 是否能重修
        *@param {string} place 上课地点
        *@param {string} note 备注
        *@param {string} teaching_material 教材
        *@param {string} campus 校区
        *@param { ScheduleTime[] } times 上课时间
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostSchedule(dynamic q)
        {
            int lesson_id = Convert.ToInt32(q.lesson_id);
            int teacher_id = Convert.ToInt32(q.teacher_id);
            int year = Convert.ToInt32(q.year);
            int half = Convert.ToInt32(q.half);
            int begin_week = Convert.ToInt32(q.begin_week);
            int end_week = Convert.ToInt32(q.end_week);
            string place = Convert.ToString(q.place);
            int max_num = Convert.ToInt32(q.max_num);
            string note = q.note != null ? Convert.ToString(q.note) : null;
            string teaching_material = q.teaching_material != null ? Convert.ToString(q.teaching_material) : null;
            int can_retake = Convert.ToInt32(q.can_retake);
            string campus = Convert.ToString(q.campus);
            List<ScheduleTime> times = q.times.ToObject<List<ScheduleTime>>();
            Schedule new_schedule = new Schedule
            {
                LessonId = lesson_id,
                TeacherId = teacher_id,
                Year = year,
                Half = half,
                BeginWeek = begin_week,
                EndWeek = end_week,
                Place = place,
                MaxNum = max_num,
                Note = note,
                TeachingMaterial = teaching_material,
                CanRetake = can_retake,
                Campus = campus
            };
            _context.Schedules.Add(new_schedule);
            try
            {
                await _context.SaveChangesAsync();
                int schedule_id = new_schedule.ScheduleId;
                foreach (ScheduleTime i in times)
                {
                    i.ScheduleId = schedule_id;
                    _context.ScheduleTimes.Add(i);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException)
            {
                if (_context.Schedules.Any())
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
