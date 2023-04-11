using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class RegeisterScoreController : ControllerBase
    {
        private readonly schoolContext _context;

        public RegeisterScoreController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/teachers/RegeisterScore
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
         *@TODO:录入成绩
         *@param {int} student_id 老师id
         *@param {string} schedule_id 密码
         *@param {string} score 验证码id
         *@param {string} status 验证码的值
         *
         *@return 
         *成功或失败都返回状态码
         *
         */
        public async Task<ActionResult> PostEnrollment(dynamic q)
        {
            var _information = await (from information in _context.Information
                                      select information).FirstOrDefaultAsync();
            DateTime dt = DateTime.Now;
            if (_information.CanImportGrade == 0 || _information.GradeBeginTime > dt || _information.GradeEndTime < dt)
                //不在规定的时间内
            {
                return BadRequest();
            }
            int student_id = Convert.ToInt32(q.student_id);
            int schedule_id = Convert.ToInt32(q.schedule_id);
            int score = Convert.ToInt32(q.score);
            string status = Convert.ToString(q.status);
            var _enroll = await (from enroll in _context.Enrollments
                                 where enroll.StudentId == student_id
                                     && enroll.ScheduleId == schedule_id
                                     && enroll.Score == null
                                 select enroll).FirstOrDefaultAsync();
            if(_enroll == null)
            {
                return BadRequest();
            }
            else
            {
                if(_enroll.Score!=null)
                {
                    return BadRequest();
                }
                DateTime now = DateTime.Now;
                
                int gradepoint;
                if (score >= 90)
                {
                    gradepoint = 5;
                }
                else if (score >= 80)
                {
                    gradepoint = 4;
                }
                else if (score >= 70)
                {
                    gradepoint = 3;
                }
                else if (score >= 60)
                {
                    gradepoint = 2;
                }
                else
                {
                    gradepoint = 0;
                }
                _enroll.Score = score;
                _enroll.GradeStatus = status;
                _enroll.InputTime = now;
                _enroll.GradePoint = gradepoint;
                try
                {
                    await _context.SaveChangesAsync();
                    LogHelper.Info("Register student:"+ student_id+"'s grades in schedule:"+ schedule_id +" successfully");
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    LogHelper.Error("Database error while Register Grade");
                    return Conflict();
                }
            }
        }
    }
}
