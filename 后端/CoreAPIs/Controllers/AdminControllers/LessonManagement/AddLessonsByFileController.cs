using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using OfficeOpenXml;

namespace CoreAPIs.Controllers.AdminControllers.LessonManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddLessonsByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddLessonsByFileController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/AddLessonsByFile
        [HttpPost]
        /*
        *@TODO:新增一堆课程信息
        *@param {IFormFile} file 文件
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostLesson([FromForm] IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                // 获取到第一个Sheet，也可以通过 Worksheets["name"] 获取指定的工作表
                var sheet = package.Workbook.Worksheets.First();


                // +1 是因为第一行往往我们获取到的都是Excel的标题
                int startRowNumber = sheet.Dimension.Start.Row + 1;
                int endRowNumber = sheet.Dimension.End.Row;
                int startColumn = sheet.Dimension.Start.Column;
                int endColumn = sheet.Dimension.End.Column;
                try
                {
                    for (int currentRow = startRowNumber; currentRow <= endRowNumber; currentRow++)
                    {
                        int i = 1;
                        string lesson_name = sheet.Cells[currentRow, i++].Text;
                        string type = sheet.Cells[currentRow, i++].Text;
                        decimal credit = Convert.ToDecimal(sheet.Cells[currentRow, i++].Text);
                        int? preq_id = sheet.Cells[currentRow, i++].Text==""?null:Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        string note = sheet.Cells[currentRow, i++].Text;
                        string need_depart = sheet.Cells[currentRow, i++].Text;
                        string identity = sheet.Cells[currentRow, i++].Text;
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
                        await _context.SaveChangesAsync();
                        int lesson_id = new_lesson.LessonId;
                        while (sheet.Cells[currentRow, i].Text != "")
                        {
                            int inyear = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            int min_grade = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            int max_grade = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            _context.LessonRequirements.Add(new LessonRequirement
                            {
                                LessonId = lesson_id,
                                InYear = inyear,
                                MinGrade = min_grade,
                                MaxGrade = max_grade
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException)
                {
                    throw;
                }
            }
            return Ok();
        }
    }
}
