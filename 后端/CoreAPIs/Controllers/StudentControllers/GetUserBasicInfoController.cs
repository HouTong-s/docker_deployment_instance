using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.StudentControllers
{
    [Route("api/students/[controller]")]
    [ApiController]
    public class GetUserBasicInfoController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetUserBasicInfoController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/students/GetUserBasicInfo
        [HttpGet]
        /*
         *@TODO:获取学生的基础信息
         *@param null
         *@return 
         *成功返回User_Basic_info对象
         *失败返回状态码
         */
        public async Task<ActionResult<User_Basic_info>> GetUserBasicInfo()
        {
            int student_id = Convert.ToInt32(Request.Headers["ids"]);
            var information = await _context.Information.FirstOrDefaultAsync();
            DateTime dt = DateTime.Now;
            TimeSpan ts= (TimeSpan)(dt - information.SemesterBeginTime);
            var name = await (from student in _context.Students
                              where student.StudentId == student_id
                              select student.StudentName).FirstOrDefaultAsync();
            if(name!=null)
            {
                return new User_Basic_info
                {
                    code = 200,
                    username = name,
                    year = information.Year,
                    half = (int)information.Half,
                    week = ts.Days / 7+1
                };
            }
            else
            {
                return NotFound();
            }
        }
    }
}
