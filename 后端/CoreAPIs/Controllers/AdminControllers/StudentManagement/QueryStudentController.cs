using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.StudentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class QueryStudentController : ControllerBase
    {
        private readonly schoolContext _context;

        public QueryStudentController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/QueryStudent
        [HttpGet]
        /*
       *@TODO:查询id对应的学生信息
       *@param {int} student_id 学生id
       *
       *@return 
       *成功返回Student对象
       *失败返回状态码
       *
       */
        public async Task<ActionResult<Student>> GetStudents([FromQuery]int student_id)
        {
            var result = await _context.Students.Where(s => s.StudentId == student_id).FirstOrDefaultAsync();
            if (result != null)
                return result;
            else
                return NotFound();
        }
    }
}
