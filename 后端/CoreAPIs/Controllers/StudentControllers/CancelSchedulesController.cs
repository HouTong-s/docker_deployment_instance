using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers.StudentControllers
{
    [Route("api/students/[controller]")]
    [ApiController]
    public class CancelSchedulesController : ControllerBase
    {
        private readonly schoolContext _context;

        public CancelSchedulesController(schoolContext context)
        {
            _context = context;
        }

        // GET: api/students/CancelSchedules
        [HttpPost]
        /*
        * @TODO:退课
	    * @param {int} schedule_id 排课id
	    * @return 成功或者失败都返回状态码
	    * 
	    * 
        * 退课失败的几个充分条件
        * 1.enrollment不存在
        * 2.不在选课时间内
        * 3.选课时间对不上(不能重修选课的时候选择退掉新修的课)
        * 4.要退的课是已经完结的的(一般为js攻击)
        * 5.数据库出现异常操作
        * 
        * 
        * 以上条件都不满足才能退课成功
        */
        public async Task<ActionResult<ResultModel>> CancelSchedules(dynamic q)
        {
            var information = await _context.Information.FirstOrDefaultAsync();
            var student_id = Convert.ToInt32(Request.Headers["ids"]);
            var SelectStatus = information.SelectStatus;

            if (SelectStatus == 0)
            {
                LogHelper.Warn("wrong time to cancel a enrollment , potential attack");
                return new ResultModel { code = 403, msg = "time err", detail = "还没到选课时间" };
            }

            int schedule_id = Convert.ToInt32(q.schedule_id);

            var _enrollment = await(from enrollment in _context.Enrollments
                                     where enrollment.ScheduleId == schedule_id 
                                     && enrollment.StudentId == student_id
                                     select enrollment).FirstOrDefaultAsync();
            
            if (_enrollment == null)
            //未查询到enrollment，直接返回
            {
                LogHelper.Warn("intend to cancel a enrollment that don't exists , potential attack");
                return NotFound();
            }

            var _schedule = await (from schedule in _context.Schedules
                                   where schedule.ScheduleId == schedule_id
                                   && schedule.IsOver == 0
                                   //必须是还没完结的课程，否则不能退课
                                   select schedule).FirstOrDefaultAsync();
            if (_schedule == null)
            //未查询到排课，说明可能是以往学期的课程，或者排课被误删除了
            {
                LogHelper.Warn("intend to cancel a enrollment that has ended , potential attack");
                return new ResultModel { code = 403, msg = "error", detail = "不能退已完结的课程" };
            }

            if (_enrollment.SelectStatus != SelectStatus)
                //确保选课时间正确
            {
                LogHelper.Warn("wrong time to cancel a enrollment , potential attack");
                return new ResultModel { code = 403, msg = "time err", detail = "选课时间错误" };
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _schedule.CurrentNum -= 1;
                _context.Entry(_schedule).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Enrollments.Remove(_enrollment);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Schedules.Any(e => e.ScheduleId == schedule_id))
                {
                    LogHelper.Error("Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateConcurrencyException ,NotFound");
                    return NotFound();
                }
                else
                {
                    LogHelper.Error("Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateConcurrencyException");
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                if (_context.Schedules.Any(e => e.ScheduleId == schedule_id))
                {
                    LogHelper.Error("Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateException , Conflict");
                    return Conflict();
                }
                else
                {
                    LogHelper.Error("Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateException ");
                    throw;
                }
            }
            catch (Exception)
            {
                LogHelper.Error("Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR Exception");
                throw;
            }
            LogHelper.Info("Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson successfully.");
            return new ResultModel { code = 204, msg = "success", detail = "退课成功" }; 
        }
    }
}
