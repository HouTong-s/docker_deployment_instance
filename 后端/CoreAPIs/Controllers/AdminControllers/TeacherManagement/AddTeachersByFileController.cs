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

namespace CoreAPIs.Controllers.AdminControllers.TeacherManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddTeachersByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddTeachersByFileController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/AddTeachersByFile
        [HttpPost]
        /*
        *@TODO:新增一堆老师
        *@param {IFormFile} file 文件
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult<ResultModel>> PostTeacher([FromForm]IFormFile file)
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
                    string teacher_name = sheet.Cells[currentRow, i++].Text;
                    string depart = sheet.Cells[currentRow, i++].Text;
                    Teacher new_teacher = new Teacher
                    {
                        TeacherId = teacher_id,
                        TeacherName = teacher_name,
                        Department = depart,
                        Salt = HashHelper.CreateVerifyCode(5)
                        //Password = teacher_id.ToString().Substring(3),
                    };
                    string str = new_teacher.TeacherId.ToString();
                    if (str.Length >= 4)
                        new_teacher.Password = HashHelper.CallMD5(new_teacher.Salt[0] + str[3..] + new_teacher.Salt[1..]);
                    else
                        new_teacher.Password = HashHelper.CallMD5(new_teacher.Salt[0] + "1234" + new_teacher.Salt[1..]);
                    if (!_context.Teachers.Any(s => s.TeacherId == teacher_id))
                    {
                        try
                        {
                            _context.Teachers.Add(new_teacher);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateException)
                        {
                            if (_context.Teachers.Any(s => s.TeacherId == teacher_id))
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
