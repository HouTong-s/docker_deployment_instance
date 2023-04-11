using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.NoticeManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddNoticeController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddNoticeController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/AddNotice
        [HttpPost]
        /*
        *@TODO:新增一条通知
        *@param {string} title 标题
        *@param {string} content 内容
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult<Notice>> PostNotice(dynamic q)
        {
            var admin_id = Convert.ToInt32(Request.Headers["ids"]);
            string title = Convert.ToString(q.title);
            string content = Convert.ToString(q.content);
            DateTime now = DateTime.Now;
            _context.Notices.Add(new Notice
            {
                AdminId = admin_id,
                Time = now,
                Title = title,
                Content = content
            });
            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException)
            {
                if (_context.Notices.Any(s => s.AdminId == admin_id && s.Time == now))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
