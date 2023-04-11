using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.Common;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.TeacherManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddTeacherController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddTeacherController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/AddTeacher
        [HttpPost]
        /*
        *@TODO:新增一个老师
        *@param {Teacher} teacher 老师对象
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostTeacher(dynamic q)
        {
            Teacher teacher = q.teacher.ToObject<Teacher>();
            if (!_context.Teachers.Any(s => s.TeacherId == teacher.TeacherId))
            {
                string str = teacher.TeacherId.ToString();
                teacher.Salt = HashHelper.CreateVerifyCode(5);
                if (str.Length >= 4)
                    teacher.Password = HashHelper.CallMD5(teacher.Salt[0] + str[3..] + teacher.Salt[1..]);
                else
                    teacher.Password = HashHelper.CallMD5(teacher.Salt[0] + "1234" + teacher.Salt[1..]);
                _context.Teachers.Add(teacher);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (_context.Teachers.Any(s => s.TeacherId == teacher.TeacherId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
        }
    }
}
