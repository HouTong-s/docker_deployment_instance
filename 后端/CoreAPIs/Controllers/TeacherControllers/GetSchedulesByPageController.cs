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
    public class GetSchedulesByPageController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetSchedulesByPageController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/teachers/GetSchedulesByPage
        [HttpGet]
        /*
         *@TODO:获取老师的某学期某页排课
         *@param {int} page 页码
         *@param {int} page_size 页面大小
         *@param {int} year 年份
         *@param {int} half 上下半年
         *@return 
         *成功返回Teacher_Schedules对象
         *失败返回状态码
         */
        public async Task<ActionResult<Teacher_Schedules>> GetSchedules([FromQuery] int page, [FromQuery] int page_size, [FromQuery] int? year, [FromQuery] int? half)
        {
            int teacher_id = Convert.ToInt32(Request.Headers["ids"]);
            List<Schedule> all;
            if(year == null || half == null)
            {
                all = await (from schedule in _context.Schedules
                             orderby schedule.ScheduleId descending
                             where schedule.TeacherId == teacher_id
                             select schedule).ToListAsync();
            }
            else
            {
                all = await (from schedule in _context.Schedules
                             orderby schedule.ScheduleId descending
                             where schedule.TeacherId == teacher_id
                                 && schedule.Year == year
                                 &&  schedule.Half == half
                             select schedule).ToListAsync();
            }   
            if (all.Count > 0)
            {
                int count = all.Count;
                int totalpage = count / page_size + 1;
                if (page > totalpage)
                {
                    return BadRequest();
                }
                else
                {
                    var results = new Teacher_Schedules();
                    results.total = count;
                    for (int i = page_size * (page - 1); i < page_size * page && i < count; i++)
                    {
                        var schedule = all[i];
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
                        var _lesson = await (from lesson in _context.Lessons
                                             where lesson.LessonId == schedule.LessonId
                                             select lesson).FirstOrDefaultAsync();
                        sche_info.lesson_name = _lesson.LessonName;
                        sche_info.credit = (decimal)_lesson.Credit;
                        results.schedules.Add(sche_info);
                    }
                    return results;
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
