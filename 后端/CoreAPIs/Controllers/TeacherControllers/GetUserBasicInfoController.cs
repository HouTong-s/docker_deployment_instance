using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class GetUserBasicInfoController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetUserBasicInfoController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/teachers/GetUserBasicInfo
        [HttpGet]
        /*
         *@TODO:获取老师的基础信息
         *@param null
         *@return 
         *成功返回User_Basic_info对象
         *失败返回状态码
         */
        public async Task<ActionResult<User_Basic_info>> GetStudents()
        {
            int teacher_id = Convert.ToInt32(Request.Headers["ids"]);
            var information = _context.Information.FirstOrDefault();
            DateTime dt = DateTime.Now;
            TimeSpan ts= (TimeSpan)(dt - information.SemesterBeginTime);
            var name = await (from teacher in _context.Teachers
                              where teacher.TeacherId == teacher_id
                              select teacher.TeacherName).FirstOrDefaultAsync();
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
