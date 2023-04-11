using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using OfficeOpenXml;

namespace CoreAPIs.Controllers.AdminControllers.TeacherManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class RegisterQuitTeachersByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public RegisterQuitTeachersByFileController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/RegisterQuitTeachersByFile
        [HttpPost]
        /*
        *@TODO:设置一堆老师离职
        *@param {IFormFile} file 文件
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostTeacher([FromForm] IFormFile file)
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

                for (int currentRow = startRowNumber; currentRow <= endRowNumber; currentRow++)
                {
                    int i = 1;
                    int teacher_id = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                    var teacher = await (from _teacher in _context.Teachers
                                         where _teacher.TeacherId == teacher_id
                                         select _teacher).FirstOrDefaultAsync();
                    teacher.IsQuit = 1;
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Teachers.Any(s => s.TeacherId == teacher_id))
                        {
                            return NotFound();
                        }
                        else
                            throw;
                    }
                }
                return Ok();
            }
        }
    }
}
