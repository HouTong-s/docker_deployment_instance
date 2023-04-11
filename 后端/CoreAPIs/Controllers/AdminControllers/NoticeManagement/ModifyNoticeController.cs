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
    public class ModifyNoticeController : ControllerBase
    {
        private readonly schoolContext _context;

        public ModifyNoticeController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/ModifyNotice
        [HttpPost]
        /*
        *@TODO:修改一条通知
        *@param {int} notice_id 通知id
        *@param {string} title 标题
        *@param {string} content 内容
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostNotice(dynamic q)
        {
            int notice_id = Convert.ToInt32(q.notice_id);
            string title = Convert.ToString(q.title);
            string content = Convert.ToString(q.content);
            var notice = await (from _notice in _context.Notices
                                where _notice.NoticeId == notice_id
                                select _notice).FirstOrDefaultAsync();
            if(notice == null)
            {
                return NotFound();
            }
            else
            {
                notice.Title = title;
                notice.Content = content;
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Notices.Any(s=>s.NoticeId == notice_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
