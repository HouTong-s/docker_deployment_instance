using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.LessonManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class GetLessonsController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetLessonsController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/GetLessons
        [HttpGet]
        /*
        *@TODO:获取对应页面的课程信息
        *@param {int} page 页码
        *@param {int} page_size 页面大小
        *
        *@return 
        *成功返回Lessons_page_info对象
        *失败返回状态码
        *
        */
        public async Task<ActionResult<Lessons_page_info>> GetLessons([FromQuery] int page,[FromQuery] int page_size)
        {
            var lessons = await _context.Lessons.OrderBy(s=>s.LessonId).ToListAsync();
            int count = lessons.Count;
            if (count > 0)
            {
                int totalpage = count / page_size + 1;
                if (page > totalpage)
                {
                    return BadRequest();
                }
                else
                {
                    Lessons_page_info result = new Lessons_page_info();
                    result.total = count;
                    for (int i = page_size * (page - 1); i < page_size * page && i < count; i++)
                    {
                        int lesson_id = lessons[i].LessonId;
                        var requires = await (from lessonRequirement in _context.LessonRequirements
                                              where lessonRequirement.LessonId == lesson_id
                                              select lessonRequirement).ToListAsync();
                        result.lessons.Add(new Lesson_requirement_info
                        {
                            lesson = lessons[i],
                            requirements = requires
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
