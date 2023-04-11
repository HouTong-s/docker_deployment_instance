using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.InfomationManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class ModifySemesterController : ControllerBase
    {
        private readonly schoolContext _context;

        public ModifySemesterController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/ModifySemester
        [HttpPost]
        /*
        *@TODO:更改学期信息
        *@param {int} year 年份
        *@param {int} half 半年
        *@param {DateTime} begintime 学期开始时间
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult<Information>> PostInformation(dynamic q)
        {
            int year = Convert.ToInt32(q.year);
            int half = Convert.ToInt32(q.half);
            DateTime begintime = Convert.ToDateTime(q.begintime);
            var information = await (from info in _context.Information
                                     select info).FirstOrDefaultAsync();
            if (information == null)
            {
                return NotFound();
            }
            else
            {
                information.Half = half;
                information.Year = year;
                information.SemesterBeginTime = begintime;
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_context.Information.FirstOrDefault() == null)
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
