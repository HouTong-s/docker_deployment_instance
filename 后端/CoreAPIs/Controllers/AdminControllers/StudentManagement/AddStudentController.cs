using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.Common;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.StudentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddStudentController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddStudentController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/AddStudent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
        *@TODO:新增一个学生
        *@param {Student} student 学生对象
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostStudent(dynamic q)
        {
            Student student = q.student.ToObject<Student>();        
            if (!_context.Students.Any(s => s.StudentId == student.StudentId))
            {
                string str = student.StudentId.ToString();
                student.Salt = HashHelper.CreateVerifyCode(5);
                if (str.Length >= 4)
                    student.Password = HashHelper.CallMD5(student.Salt[0] + str.Substring(3) + student.Salt[1..]);
                else
                    student.Password = HashHelper.CallMD5(student.Salt[0] + "1234" + student.Salt[1..]);
                _context.Students.Add(student);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (_context.Students.Any(s => s.StudentId == student.StudentId))
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
