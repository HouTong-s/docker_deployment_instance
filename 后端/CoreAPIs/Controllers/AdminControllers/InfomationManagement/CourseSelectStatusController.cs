using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.InfomationManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class CourseSelectStatusController : ControllerBase
    {
        private readonly schoolContext _context;

        public CourseSelectStatusController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/admins/CourseSelectStatus
        [HttpGet]
        /*
        *@TODO:获取选课状态
        *@param null
        *
        *@return 
        *成功返回SelectStatus对象
        *失败返回状态码404
        */
        public async Task<ActionResult<SelectStatus>> GetStatus()
        {
            var information = await (from _information in _context.Information
                                     select _information).FirstOrDefaultAsync();
            if (information == null)
            {
                return NotFound();
            }
            return new SelectStatus
            {
                beginTime = (DateTime)information.SelectBeginTime,
                status = information.SelectStatus,
                endTime = (DateTime)information.SelectEndTime,
            };
        }
        // POST: api/admins/CourseSelectStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
        *@TODO:更改选课状态
        *@param {int} status 新的状态
        *@param {DateTime?} selectBeginTime 选课开始时间
        *@param {DateTime?} selectEndTime 选课结束时间
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostInformation(dynamic q)
        {
            int status = Convert.ToInt32(q.status);
            DateTime? selectBeginTime = q.selectBeginTime == null ? null : Convert.ToDateTime(q.selectBeginTime);
            DateTime? selectEndTime = q.selectEndTime == null ? null : Convert.ToDateTime(q.selectEndTime);
            var information = await (from _information in _context.Information
                                     select _information).FirstOrDefaultAsync();
            information.SelectStatus = status;
            information.SelectBeginTime = selectBeginTime;
            information.SelectEndTime = selectEndTime;
            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                var temp = await _context.Information.Select(s => s).FirstOrDefaultAsync();
                if (temp == null)
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
