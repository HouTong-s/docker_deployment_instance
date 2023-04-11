using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.StudentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class GetStudentsController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetStudentsController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/admins/GetStudents
        [HttpGet]
        /*
        *@TODO:获取对应页面的学生信息
        *@param {int} page 页码
        *@param {int} page_size 页面大小
        *
        *@return 
        *成功返回students_page_info对象
        *失败返回状态码
        *
        */
        public async Task<ActionResult<students_page_info>> GetStudents([FromQuery] int page, [FromQuery] int page_size)
        {
            var students = await _context.Students.OrderBy(s => s.StudentId).ToListAsync();
            int count = students.Count;
            if (count > 0)
            {
                int totalpage = count / page_size + 1;
                if (page > totalpage)
                {
                    return BadRequest();
                }
                else
                {
                    students_page_info result = new students_page_info();
                    result.total = count;
                    for (int i = page_size * (page - 1); i < page_size * page && i < count; i++)
                    {
                        result.students.Add(students[i]);
                    }
                    return result;
                }
            }
            else
                return NotFound();
        }
    }
}
