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
    public class QueryTeacherController : ControllerBase
    {
        private readonly schoolContext _context;

        public QueryTeacherController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/QueryTeacher
        [HttpGet]
        /*
       *@TODO:查询id对应的老师信息
       *@param {int} teacher_id 老师id
       *
       *@return 
       *成功返回Teacher对象
       *失败返回状态码
       *
       */
        public async Task<ActionResult<Teacher>> GetTeachers([FromQuery]int teacher_id)
        {
            var result = await _context.Teachers.Where(s => s.TeacherId == teacher_id).FirstOrDefaultAsync();
            if (result != null)
                return result;
            else
                return NotFound();
        }
    }
}
