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
    public class SelectSchedulesController : ControllerBase
    {
        private readonly schoolContext _context;

        public SelectSchedulesController(schoolContext context)
        {
            _context = context;
        }
        // api/students/SelectSchedules
        [HttpPost]
        /*
         *@TODO:选课
	     *@param {int} schedule_id 排课id(json字符串)
	     *@return 成功或者失败都返回状态码
         * 
         * q为一个json对象，只有schedule_id一个字段作为要选排课的id
         * 
         * 选课失败的几个充分条件：
         * 1.排课(schedule)不存在
         * 2.排课人数已满
         * 3.没有选课资格
         * 4.(第一次选或者重修)已经选了该lesson的课
         * 5.重修选课时，该lesson还没有修过
         * 6.和本学期已有的课程时间相冲突
         * 7.如果在选课时出现了数据库的异常操作(比如同时进行的更新操作)
         * 
         * 只有以上条件都不满足才能选课成功
         */
        public async Task<ActionResult<ResultModel>> SelectSchedule(dynamic q)
        {
            var information = await _context.Information.FirstOrDefaultAsync();
            var SelectStatus = information.SelectStatus;
            if (SelectStatus == 0)
            {
                LogHelper.Warn("wrong time to take a schedule , potential attack");
                return new ResultModel { code = 409, msg = "time err", detail = "还没到选课时间" };
            }
            int schedule_id = Convert.ToInt32(q.schedule_id);
            var _schedule = await (from schedule in _context.Schedules
                                   where schedule.ScheduleId == schedule_id
                                   select schedule).FirstOrDefaultAsync();
            var student_id = Convert.ToInt32(Request.Headers["ids"]);
            var lesson_id = _schedule.LessonId;
            if (_schedule == null)
            //未查询到排课，直接返回
            {
                LogHelper.Warn("intend to take a schedule that don't exists , potential attack");
                return NotFound();
            }
            if (_schedule.CurrentNum >= _schedule.MaxNum)
            //人员已满拒绝选课
            {
                return new ResultModel { code = 409, msg = "full", detail = "该课程人员已满" };
            }
            //以下判断是否有选课的资格
            var student_info = await (from student in _context.Students
                                      where student.StudentId == student_id
                                      select student).FirstOrDefaultAsync();
            int grade = (int)(information.Year - student_info.InYear + information.Half);
            var require = await (from req in
                               (from lesson_requirement in _context.LessonRequirements
                                where lesson_requirement.LessonId == lesson_id
                                       && lesson_requirement.InYear == student_info.InYear
                                       && lesson_requirement.MinGrade <= grade
                                       && grade <= lesson_requirement.MaxGrade
                                select lesson_requirement)
                                 join lesson in _context.Lessons
                                 on req.LessonId equals lesson.LessonId
                                 where lesson.Identity == student_info.Identity
                                 && (lesson.NeedDepart == student_info.Department || lesson.NeedDepart == "all")
                                 //满足身份，年级，哪一年入学才能选择该课
                                 select lesson).FirstOrDefaultAsync();
            if (require == null)
            {
                LogHelper.Warn("intend to take a not qualified schedule ,potential attack");
                return new ResultModel { code = 409, msg = "not qualified", detail = "没有选课资格" };
            }
            //获取已经修过的课程
            var selected_lessons_id = await (from schedule in _context.Schedules
                                             join enroll in
                                             (from enrollment in _context.Enrollments
                                              where enrollment.StudentId == student_id
                                              select enrollment)
                                             on schedule.ScheduleId equals enroll.ScheduleId
                                             select new
                                             {
                                                 lesson_id = schedule.LessonId,
                                                 is_over = schedule.IsOver
                                             }).ToListAsync();
            if (SelectStatus == 1)//正常选课时，选过的lesson不能再选(以前的，这学期的)
            {
                if (selected_lessons_id.Select(s => s.lesson_id).Contains(lesson_id))
                {
                    LogHelper.Warn("intend to take a studyed schedule ,potential attack");
                    return new ResultModel { code = 409, msg = "studyed", detail = "该课程已修过，或者这学期已选" };
                }
            }
            else//重修选课时，只有这学期以前修过的课才能选(所以要is_over =1)
            {
                if (!selected_lessons_id.Where(s => s.is_over == 1).Select(s => s.lesson_id).Contains(lesson_id))
                {
                    LogHelper.Warn("intend to retake a new schedule ,potential attack");
                    return new ResultModel { code = 409, msg = "not studyed", detail = "该课程还未修过，无法重修" };
                }
                else if (selected_lessons_id.Where(s => s.is_over == 0).Select(s => s.lesson_id).Contains(lesson_id))
                {
                    LogHelper.Warn("intend to retake a studying schedule ,potential attack");
                    return new ResultModel { code = 409, msg = "studying", detail = "该课程这学期已选" };
                }
            }
            //以下判断是否和本学期已有的课程时间相冲突
            var current_schedule_times_info = new ScheduleTimes_Info
            {
                begin_week = (int)_schedule.BeginWeek,
                end_week = (int)_schedule.EndWeek,
                times = new List<SingleTime>()
            };
            var curent_times = await (from schedule_time in _context.ScheduleTimes
                                      where schedule_time.ScheduleId == _schedule.ScheduleId
                                      select schedule_time).ToListAsync();
            foreach (ScheduleTime time in curent_times)
            {
                current_schedule_times_info.times.Add(new SingleTime
                {
                    begin_section = time.BeginSection,
                    end_section = (int)time.EndSection,
                    single_or_double = (int)time.SingleOrDouble,
                    dayweek = time.DayWeek
                });
            }
            var semester_all_schedules = await(from enroll in
                                              (from enrollment in _context.Enrollments
                                               where enrollment.StudentId == student_id
                                               select enrollment)
                                                join schedule in _context.Schedules
                                                on enroll.ScheduleId equals schedule.ScheduleId
                                                where schedule.Year == information.Year && schedule.Half == information.Half
                                                select new
                                                {
                                                    schedule_id = schedule.ScheduleId,
                                                    begin_week = schedule.BeginWeek,
                                                    end_week = schedule.EndWeek
                                                }).ToListAsync();
            var all_times = new List<ScheduleTimes_Info>();
            foreach (var sche in semester_all_schedules)
            {
                var times = await (from scheduletimes in _context.ScheduleTimes
                                   where scheduletimes.ScheduleId == sche.schedule_id
                                   select new SingleTime
                                   {
                                       begin_section = scheduletimes.BeginSection,
                                       end_section = (int)scheduletimes.EndSection,
                                       dayweek = scheduletimes.DayWeek,
                                       single_or_double = (int)scheduletimes.SingleOrDouble
                                   }
                                   ).ToListAsync();
                var temp = new ScheduleTimes_Info
                {
                    begin_week = (int)sche.begin_week,
                    end_week = (int)sche.end_week,
                    times = times
                };
                all_times.Add(temp);
            }
            if (all_times.Count > 0)
            {
                if (ScheduleTimeJudgement.IsConflict(current_schedule_times_info, all_times))
                {
                    return new ResultModel { code = 409, msg = "time confict", detail = "时间冲突，选课失败" };
                }
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _schedule.CurrentNum += 1;
                _context.Entry(_schedule).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _context.Enrollments.Add(new Enrollment
                {
                    GradeStatus = "未出成绩",
                    ScheduleId = schedule_id,
                    StudentId = student_id,
                    SelectStatus = SelectStatus
                });
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Schedules.Any(e => e.ScheduleId == schedule_id))
                {
                    LogHelper.Error("Student:" + student_id + " selected schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateConcurrencyException ,NotFound");
                    return NotFound();
                }
                else
                {
                    LogHelper.Error("Student:" + student_id + " selected schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateConcurrencyException");
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                if (_context.Schedules.Any(e => e.ScheduleId == schedule_id))
                {
                    LogHelper.Error("Student:" + student_id + " selected schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateException , Conflict");
                    return Conflict();
                }
                else
                {
                    LogHelper.Error("Student:" + student_id + " selected schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR DbUpdateException ");
                    throw;
                }
            }
            catch (Exception)
            {
                LogHelper.Error("Student:" + student_id + " selected schedule_id:" + schedule_id + " lesson unsuccessfully. ERROR Exception");
                throw;
            }
            LogHelper.Info("Student:" + student_id + " selected schedule_id:" + schedule_id + " lesson successfully.");
            return new ResultModel { code = 204, msg = "success", detail = "选课成功" }; ;
        }
    }
}
/*
            
 */

/*
 * 以下为一段多余的代码，但也有一定的意义
    //如果已经选过了这一学期某一个老师(当前schedule对应的老师，或者另一个老师)对应的同一个lesson的话也不行（算另一种重复选课）
    var another_enroll =   (from enrollment in _context.Enrollments
                           join sche in 
                         (from schedule in _context.Schedules
                          where schedule.LessonId == lesson_id && schedule.Year == information.Year && schedule.Half == information.Half
                          select schedule) on enrollment.ScheduleId equals sche.ScheduleId 
                          where enrollment.StudentId == student_id 
                          select enrollment).FirstOrDefault();
    if (another_enroll != null)
    //已经选了该lesson的相关schedule，拒绝重复选择
    {
        return new ResultModel { code = 409, msg = "repeat", detail = "请勿重复选课" };
    }
*/