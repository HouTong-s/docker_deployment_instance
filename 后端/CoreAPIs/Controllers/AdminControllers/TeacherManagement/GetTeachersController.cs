using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.TeacherManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class GetTeachersController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetTeachersController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/GetTeachers
        [HttpGet]
        /*
        *@TODO:获取对应页面的老师信息
        *@param {int} page 页码
        *@param {int} page_size 页面大小
        *
        *@return 
        *成功返回teachers_page_info对象
        *失败返回状态码
        *
        */
        public async Task<ActionResult<teachers_page_info>> GetStudents([FromQuery] int page, [FromQuery] int page_size)
        {
            var teachers = await _context.Teachers.OrderBy(s => s.TeacherId).ToListAsync();
            int count = teachers.Count;
            if (count > 0)
            {
                int totalpage = count / page_size + 1;
                if (page > totalpage)
                {
                    return BadRequest();
                }
                else
                {
                    teachers_page_info result = new teachers_page_info();
                    result.total = count;
                    for (int i = page_size * (page - 1); i < page_size * page && i < count; i++)
                    {
                        result.teachers.Add(teachers[i]);
                    }
                    return result;
                }
            }
            else
                return NotFound();
        }
    }
}
