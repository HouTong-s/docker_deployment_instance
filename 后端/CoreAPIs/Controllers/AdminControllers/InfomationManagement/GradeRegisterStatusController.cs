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
    public class GradeRegisterStatusController : ControllerBase
    {
        private readonly schoolContext _context;

        public GradeRegisterStatusController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/admins/GradeRegisterStatus
        [HttpGet]
        /*
        *@TODO:获取上传成绩的状态
        *@param null
        *
        *@return 
        *成功返回GradeImportStatus对象
        *失败返回状态码404
        */
        public async Task<ActionResult<GradeImportStatus>> GetStatus()
        {
            var information = await (from _information in _context.Information
                                     select _information).FirstOrDefaultAsync();
            if(information == null)
            {
                return NotFound();
            }
            return new GradeImportStatus
            {
                beginTime = (DateTime)information.GradeBeginTime,
                status = information.CanImportGrade,
                endTime = (DateTime)information.GradeEndTime,
            };
        }
        // POST: api/admins/GradeRegisterStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
        *@TODO:更改上传成绩的状态
        *@param {int} status 新的状态
        *@param {DateTime?} gradeBeginTime 上传的开始时间
        *@param {DateTime?} gradeEndTime 上传的结束时间
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostInformation(dynamic q)
        {
            int status = Convert.ToInt32(q.status);
            DateTime? gradeBeginTime = q.gradeBeginTime == null ? null : Convert.ToDateTime(q.gradeBeginTime);
            DateTime? gradeEndTime = q.gradeEndTime == null ? null : Convert.ToDateTime(q.gradeEndTime);
            var information = await (from _information in _context.Information
                                     select _information).FirstOrDefaultAsync();
            information.CanImportGrade = status;
            information.GradeBeginTime = gradeBeginTime;
            information.GradeEndTime = gradeEndTime;
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
