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

namespace CoreAPIs.Controllers.AdminControllers.EnrollmentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class CancelScheduleForStudentController : ControllerBase
    {
        private readonly schoolContext _context;

        public CancelScheduleForStudentController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/CancelScheduleForStudent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
        *@TODO:代学生退课
        *@param {int} schedule_id 排课id
        *@param {int} student_id 学生id
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult<ResultModel>> PostSchedule(dynamic q)
        {
            int schedule_id = Convert.ToInt32(q.schedule_id);
            int student_id = Convert.ToInt32(q.student_id);
            var information = await _context.Information.FirstOrDefaultAsync();
            var SelectStatus = information.SelectStatus;

            var _enrollment = await (from enrollment in _context.Enrollments
                                     where enrollment.ScheduleId == schedule_id
                                     && enrollment.StudentId == student_id
                                     select enrollment).FirstOrDefaultAsync();

            if (_enrollment == null)
            //未查询到enrollment，直接返回
            {
                Console.WriteLine("intend to cancel a enrollment that don't exists , potential attack");
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
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _schedule.CurrentNum -= 1;
                await _context.SaveChangesAsync();
                _context.Enrollments.Remove(_enrollment);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Schedules.Any(e => e.ScheduleId == schedule_id))
                {
                    LogHelper.Error("Admin Help for Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateConcurrencyException ,NotFound");
                    return NotFound();
                }
                else
                {
                    LogHelper.Error("Admin Help for Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateConcurrencyException");
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                if (_context.Schedules.Any(e => e.ScheduleId == schedule_id))
                {
                    LogHelper.Error("Admin Help for Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateException , Conflict");
                    return Conflict();
                }
                else
                {
                    LogHelper.Error("Admin Help for Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateException ");
                    throw;
                }
            }
            catch (Exception)
            {
                LogHelper.Error("Admin Help for Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR Exception");
                throw;
            }
            LogHelper.Info("Admin Help for Student:" + student_id + " cancelled schedule_id:" + schedule_id + " lesson successfully.");
            return new ResultModel { code = 204, msg = "success", detail = "退课成功" };
        }
    }
}
