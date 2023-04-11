using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.ScheduleManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class EndSchedulesController : ControllerBase
    {
        private readonly schoolContext _context;

        public EndSchedulesController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/EndSchedules
        [HttpGet]
        /*
        *@TODO:获取本学期是否已完结
        *@param null
        *
        *@return 
        *返回1表示本学期已完结
        *返回0表示本学期未完结
        *
        */
        public async Task<ActionResult<int>> Get()
        {
            var information = await (from info in _context.Information
                                     select info).FirstOrDefaultAsync();
            var a = await (from schedules in _context.Schedules
                           where schedules.Year == information.Year
                               && schedules.Half == information.Half
                           select schedules).FirstOrDefaultAsync();
            if(a==null)
            {
                return 0;
            }
            else
            {
                if(a.IsOver==0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
        [HttpPost]
        /*
        *@TODO:本学期结课
        *@param null
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostSchedule()
        {
            var information = await (from info in _context.Information
                                     select info).FirstOrDefaultAsync();
            DateTime now = DateTime.Now;
            TimeSpan ts = now - information.SemesterBeginTime;
            if(ts.Days <= 17*7)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("update `schedule` set `is_over` = 1 where `year` = " + information.Year + " and `half` = " + information.Half);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return Ok();
            }
            
        }
    }
}
