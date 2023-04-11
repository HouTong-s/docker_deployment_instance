using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.Models;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.ScheduleManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class ModifyScheduleController : ControllerBase
    {
        private readonly schoolContext _context;

        public ModifyScheduleController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/ModifySchedule    
        [HttpPost]
        /*
        *@TODO:更改一条排课信息
        *@param {int} begin_week 开始周
        *@param {int} end_week 结束周
        *@param {int} max_num 最多人数
        *@param {int} schedule_id 更改的排课对应id
        *@param {string} place 上课地点
        *@param {string} note 备注
        *@param {string} teaching_material 教材
        *@param { ScheduleTime[] } times 上课时间
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostSchedule(dynamic q)
        {
            string place = Convert.ToString(q.place);
            int maxNum = Convert.ToInt32(q.maxNum);
            int begin_week = Convert.ToInt32(q.begin_week);
            int end_week = Convert.ToInt32(q.end_week);
            string? note = q.note != null ? Convert.ToString(q.note):null;
            string? teachingMaterial = q.teachingMaterial != null ? Convert.ToString(q.teachingMaterial) : null;
            int schedule_id = Convert.ToInt32(q.schedule_id);
            List<ScheduleTime> times = q.times.ToObject<List<ScheduleTime>>();
            Schedule temp_schedule = await (from _schedule in _context.Schedules
                                            where _schedule.ScheduleId == schedule_id
                                            select _schedule).FirstOrDefaultAsync();
            if(temp_schedule == null)
            {
                return NotFound();
            }
            temp_schedule.Place = place;
            temp_schedule.MaxNum = maxNum;
            temp_schedule.Note = note;
            temp_schedule.TeachingMaterial = teachingMaterial;
            temp_schedule.BeginWeek = begin_week;
            temp_schedule.EndWeek = end_week;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Schedules.Any(s=>s.ScheduleId == schedule_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var old_times = await (from time in _context.ScheduleTimes
                                      where time.ScheduleId == schedule_id
                                   select time).ToListAsync();
            try
            {
                foreach (var i in old_times)
                {
                    _context.ScheduleTimes.Remove(i);
                }
                await _context.SaveChangesAsync();
                foreach (ScheduleTime i in times)
                {
                    i.ScheduleId = schedule_id;
                    _context.ScheduleTimes.Add(i);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException)
            {
                if (_context.Schedules.Any(s => s.ScheduleId == schedule_id))
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
