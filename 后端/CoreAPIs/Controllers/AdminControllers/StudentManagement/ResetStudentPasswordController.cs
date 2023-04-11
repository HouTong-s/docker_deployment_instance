using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers.AdminControllers.StudentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class ResetStudentPasswordController : ControllerBase
    {
        private readonly schoolContext _context;

        public ResetStudentPasswordController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/ResetStudentPassword
        [HttpPost]
        /*
        *@TODO:重置一名学生的密码
        *@param {int} student_id 学生id
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostStudent(dynamic q)
        {
            int student_id = Convert.ToInt32(q.student_id);
            var student = await _context.Students.Where(s => s.StudentId == student_id).FirstOrDefaultAsync();
            if (student != null)
            {
                string pwd;
                if(student.StudentId >=4)
                {
                    pwd = student.StudentId.ToString().Substring(3);
                }
                else
                {
                    pwd = "1234";
                }
                student.Password = HashHelper.CallMD5(student.Salt[0] + pwd + student.Salt[1..]);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(s => s.StudentId == student_id))
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
