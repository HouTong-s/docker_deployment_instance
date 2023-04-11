using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using OfficeOpenXml;
using CoreAPIs.Models;

namespace CoreAPIs.Controllers.AdminControllers.StudentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class RegisterGraduateStudentsByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public RegisterGraduateStudentsByFileController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/admins/RegisterGraduateStudentsByFile
        [HttpPost]
        /*
        *@TODO:设置一堆学生毕业
        *@param {IFormFile} file 文件
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> PostStudent([FromForm] IFormFile file)
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
                    int student_id = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                    var student = await (from _student in _context.Students
                                         where _student.StudentId == student_id
                                         select _student).FirstOrDefaultAsync();
                    student.IsGraduate = 1;
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Students.Any(s => s.StudentId == student_id))
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
