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
    public class ModifyLessonController : ControllerBase
    {
        private readonly schoolContext _context;

        public ModifyLessonController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/ModifyLesson
        [HttpPost]
        /*
        *@TODO:更改一条课程信息
        *@param {string} lesson_name 课程名
        *@param {string} type 课程类型
        *@param {string} note 备注
        *@param {string} need_depart 可选的专业
        *@param { LessonRequirement[] } requires 选课要求
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostLesson(dynamic q)
        {
            int lesson_id = Convert.ToInt32(q.lesson_id);
            var lesson = await (from _lesson in _context.Lessons
                                where _lesson.LessonId == lesson_id
                                select _lesson).FirstOrDefaultAsync();
            if(lesson == null)
            {
                return NotFound();
            }
            else
            {
                string lesson_name = Convert.ToString(q.lesson_name);
                string type = Convert.ToString(q.type);
                //decimal credit = Convert.ToDecimal(q.credit);
                //int? preq_id = Convert.ToInt32(q.preq_id);
                string? note = q.note!=null?Convert.ToString(q.note):null;
                string need_depart = Convert.ToString(q.need_depart);
                List<LessonRequirement> requires = q.requires.ToObject<List<LessonRequirement>>();
                lesson.LessonName = lesson_name;
                lesson.Type = type;
                //lesson.Credit = credit;
                //lesson.PreqId = preq_id;
                lesson.Note = note;
                lesson.NeedDepart = need_depart;
                //lesson.Identity = identity;
                var old_requires = await (from _require in _context.LessonRequirements
                                      where _require.LessonId == lesson_id
                                      select _require).ToListAsync();
                try
                {
                    foreach (var i in old_requires)
                    {
                        _context.LessonRequirements.Remove(i);
                    }
                    await _context.SaveChangesAsync();
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
                    if (_context.Lessons.Any(s => s.LessonId == lesson_id))
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
}
