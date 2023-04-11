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
    public class GetNoticesController : ControllerBase
    {
        private readonly schoolContext _context;

        public GetNoticesController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/teachers/GetNotices
        [HttpGet]
        /*
         *@TODO:获取的指定页面大小的页码的通知
         *@param {int} page 页码
         *@param {int} page_size 页面大小
         *@return 
         *成功返回Notice_info对象
         *失败返回状态码
         */
        public async Task<ActionResult<Notice_info>> GetNotices([FromQuery] int page, [FromQuery] int page_size)
        {
            var result = await _context.Notices.OrderByDescending(s => s.Time).ToListAsync();
            
            if (result.Count > 0)
            {
                int count = result.Count;
                int totalpage = count / page_size + 1;
                if (page > totalpage)
                {
                    return BadRequest();
                }
                else
                {
                    Notice_info page_result = new Notice_info();
                    page_result.total = result.Count;
                    for (int i = page_size * (page - 1); i < page_size * page && i < count; i++)
                    {
                        page_result.notices.Add(new single_notice
                        {
                            Title = result[i].Title,
                            Time = result[i].Time,
                            Content = result[i].Content
                        });
                    }
                    return page_result;
                }
            }
            else
                return NotFound();
        }

    }
}
