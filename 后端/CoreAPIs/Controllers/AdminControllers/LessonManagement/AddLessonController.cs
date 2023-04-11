using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;

namespace CoreAPIs.Controllers.AdminControllers.LessonManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddLessonController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddLessonController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/AddLesson
        [HttpPost]
        /*
        *@TODO:新增一条课程信息
        *@param {string} lesson_name 课程名
        *@param {string} type 课程类型
        *@param {string} note 备注
        *@param {string} need_depart 可选的专业
        *@param {string} identity 属于本、硕或博的课程
        *@param {int} preq_id 以往的课程id
        *@param {decimal} credit 学分
        *@param { LessonRequirement[] } requires 选课要求
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> AddLesson(dynamic q)
        {
            string lesson_name = Convert.ToString(q.lesson_name);
            string type = Convert.ToString(q.type);
            decimal credit = Convert.ToDecimal(q.credit);
            int? preq_id = q.preq_id == null ? null : Convert.ToInt32(q.preq_id);
            string note = q.note == null ? null : Convert.ToString(q.note);
            string need_depart = Convert.ToString(q.need_depart);
            string identity = Convert.ToString(q.identity);
            List<LessonRequirement> requires = q.requires.ToObject<List<LessonRequirement>>();
            Lesson new_lesson = new Lesson
            {
                LessonName = lesson_name,
                Type = type,
                Credit = credit,
                PreqId = preq_id,
                Note = note,
                NeedDepart = need_depart,
                Identity = identity
            };
            _context.Lessons.Add(new_lesson);
            try
            {
                await _context.SaveChangesAsync();
                int lesson_id = new_lesson.LessonId;
                foreach (LessonRequirement i in requires)
                {
                    i.LessonId = lesson_id;
                    _context.LessonRequirements.Add(i);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException)
            {
                if (_context.Schedules.Any())
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
