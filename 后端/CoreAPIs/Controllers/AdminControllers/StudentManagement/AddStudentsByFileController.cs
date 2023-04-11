using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.Common;
using CoreAPIs.DbModels;
using CoreAPIs.Models;
using OfficeOpenXml;

namespace CoreAPIs.Controllers.AdminControllers.StudentManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddStudentsByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddStudentsByFileController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/adminds/AddStudentsByFile
        [HttpPost]
        /*
        *@TODO:新增一堆学生
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
                    string student_name = sheet.Cells[currentRow, i++].Text;
                    string depart = sheet.Cells[currentRow, i++].Text;
                    int in_year = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                    string identity = sheet.Cells[currentRow, i++].Text;
                    Student new_student = new Student
                    {
                        StudentId = student_id,
                        StudentName = student_name,
                        Department = depart,
                        OriginInyear = in_year,
                        InYear = in_year,
                        Salt = HashHelper.CreateVerifyCode(5),
                        //Password = student_id.ToString().Substring(3),
                        Identity = identity
                    };
                    string str = new_student.StudentId.ToString();
                    if (str.Length >= 4)
                        new_student.Password = HashHelper.CallMD5(new_student.Salt[0] + str.Substring(3) + new_student.Salt[1..]);
                    else
                        new_student.Password = HashHelper.CallMD5(new_student.Salt[0] + "1234" + new_student.Salt[1..]);
                    if (!_context.Students.Any(s=>s.StudentId==student_id))
                    {
                        try
                        {
                            _context.Students.Add(new_student);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateException)
                        {
                            if (_context.Students.Any(s => s.StudentId == student_id))
                            {
                                return Conflict();
                            }
                            else
                                throw;
                        }
                    }
                }
                return Ok();
            }
        }
    }
}
