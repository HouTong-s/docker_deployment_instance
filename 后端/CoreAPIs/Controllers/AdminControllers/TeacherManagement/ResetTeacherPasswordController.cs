using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers.AdminControllers.TeacherManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class ResetTeacherPasswordController : ControllerBase
    {
        private readonly schoolContext _context;

        public ResetTeacherPasswordController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/ResetTeacherPassword
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
        *@TODO:重置一名老师的密码
        *@param {int} teacher_id 老师id
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostTeacher(dynamic q)
        {
            int teacher_id = Convert.ToInt32(q.teacher_id);
            var teacher = await _context.Teachers.Where(s => s.TeacherId == teacher_id).FirstOrDefaultAsync();
            if (teacher != null)
            {
                string pwd;
                if (teacher.TeacherId >= 4)
                {
                    pwd = teacher.TeacherId.ToString().Substring(3);
                }
                else
                {
                    pwd = "1234";
                }
                teacher.Password = HashHelper.CallMD5(teacher.Salt[0] + pwd + teacher.Salt[1..]);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teachers.Any(s=>s.TeacherId == teacher_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            else
                return NotFound();
        }
    }
}
