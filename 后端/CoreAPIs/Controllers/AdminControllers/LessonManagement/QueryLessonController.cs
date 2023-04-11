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
    public class QueryLessonController : ControllerBase
    {
        private readonly schoolContext _context;

        public QueryLessonController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/QueryLesson
        [HttpGet]
        /*
        *@TODO:查询对应id的课程信息
        *@param {int} lesson_id 课程id
        *
        *@return 
        *成功就返回Lesson_requirement_info对象
        *失败返回状态码
        *
        */
        public async Task<ActionResult<Lesson_requirement_info>> GetLessons([FromQuery] int lesson_id)
        {
            var lesson = await (from _lesson in _context.Lessons
                                where _lesson.LessonId == lesson_id
                                select _lesson).FirstOrDefaultAsync();
            var requires = await (from require in _context.LessonRequirements
                                  where require.LessonId == lesson_id
                                  select require).ToListAsync();
            if(lesson == null)
            {
                return NotFound();
            }
            else
            {
                return new Lesson_requirement_info{
                    lesson = lesson,
                    requirements = requires
                };
            }
        }
    }
}
