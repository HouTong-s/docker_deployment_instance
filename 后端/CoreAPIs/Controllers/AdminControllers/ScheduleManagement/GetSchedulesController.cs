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
    public class GetSchedulesController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetSchedulesController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/GetSchedules
        [HttpGet]
        /*
        *@TODO:获取对应页面的排课信息
        *@param {int} page 页码
        *@param {int} page_size 页面大小
        *
        *@return 
        *成功返回Schedules_page_info对象
        *失败返回状态码
        *
        */
        public async Task<ActionResult<Schedules_page_info>> GetSchedules([FromQuery] int page, [FromQuery] int page_size)
        {
            var schedules = await _context.Schedules.OrderBy(s => s.ScheduleId).ToListAsync();
            int count = schedules.Count;
            if (count > 0)
            {
                int totalpage = count / page_size + 1;
                if (page > totalpage)
                {
                    return BadRequest();
                }
                else
                {
                    Schedules_page_info result = new Schedules_page_info();
                    result.total = count;
                    for (int i = page_size * (page - 1); i < page_size * page && i < count; i++)
                    {
                        int schedule_id = schedules[i].ScheduleId;
                        var _times = await (from times in _context.ScheduleTimes
                                              where times.ScheduleId == schedule_id
                                              select times).ToListAsync();
                        var _lesson = await (from lessson in _context.Lessons
                                                where lessson.LessonId == schedules[i].LessonId
                                                select lessson).FirstOrDefaultAsync();
                        var teacher_name = await (from teacher in _context.Teachers
                                                  where teacher.TeacherId == schedules[i].TeacherId
                                                  select teacher.TeacherName).FirstOrDefaultAsync();
                        result.schedules.Add(new schedule_times_info
                        {
                            schedule = schedules[i],
                            teacher_name = teacher_name,
                            lesson_name = _lesson.LessonName,
                            credit = _lesson.Credit,
                            times = _times
                        });
                    }
                    return result;
                }
            }
            else
                return NotFound();
        }
    }
}
