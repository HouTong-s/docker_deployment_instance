using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.TeacherManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class RegisterQuitTeacherController : ControllerBase
    {
        private readonly schoolContext _context;

        public RegisterQuitTeacherController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/RegisterQuitTeacher
        [HttpPost]
        /*
        *@TODO:设置一名老师离职
        *@param {int} teacher_id 老师id
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostTeacher(dynamic q)
        {
            int teacher_id = Convert.ToInt32(q.teacher_id);
            var teacher = await (from _teacher in _context.Teachers
                                 where _teacher.TeacherId == teacher_id
                                 select _teacher).FirstOrDefaultAsync();
            if(teacher !=null)
            {
                try
                {
                    teacher.IsQuit = 1;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teachers.Any(s => s.TeacherId == teacher_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return NotFound();}
            
        }
    }
}
