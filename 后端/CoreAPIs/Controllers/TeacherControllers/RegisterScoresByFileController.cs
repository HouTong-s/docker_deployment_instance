using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using OfficeOpenXml;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class RegisterScoresByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public RegisterScoresByFileController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/teachers/RegisterScoresByFile
        [HttpPost]
        /*
         *@TODO:根据上传的文件录入成绩
         *@param {IFormFile} file 文件
         *@param {int} schedule_id 排课号
         *@return 
         *成功返回状态码
         *失败返回状态码
         */
        public async Task<ActionResult> PostSchedule([FromForm] IFormFile file,[FromForm] int schedule_id)
        {
            var _information = await (from information in _context.Information
                                     select information).FirstOrDefaultAsync();
            DateTime dt = DateTime.Now;
            if(_information.CanImportGrade == 0 ||_information.GradeBeginTime > dt || _information.GradeEndTime < dt)
                //不在规定的时间内
            {
                return BadRequest();
            }
            var _schedule = await (from schedule in _context.Schedules
                                   where schedule.ScheduleId == schedule_id
                                   select schedule).FirstOrDefaultAsync();
            Console.WriteLine(_schedule.ScheduleId);
            if(_schedule == null)
            {
                return NotFound();
            }
            else if(_schedule.IsOver == 1)
            {
                return BadRequest();
            }
            //_schedule.IsOver = 1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                // 获取到第一个Sheet，也可以通过 Worksheets["name"] 获取指定的工作表
                var sheet = package.Workbook.Worksheets.First();
                #region 获取开始和结束行列的个数

                var all_students = await (from enrollment in _context.Enrollments
                                          where enrollment.ScheduleId == schedule_id
                                          select enrollment).ToListAsync();
                // +1 是因为第一行往往我们获取到的都是Excel的标题
                int startRowNumber = sheet.Dimension.Start.Row + 1;
                int endRowNumber = sheet.Dimension.End.Row;
                int startColumn = sheet.Dimension.Start.Column;
                int endColumn = sheet.Dimension.End.Column;
                Console.WriteLine(endColumn);
                DateTime now = DateTime.Now;
                #endregion
                // 循环获取整个Excel数据表数据
                for (int currentRow = startRowNumber; currentRow <= endRowNumber; currentRow++)
                {
                    Console.WriteLine(sheet.Cells[currentRow, 1].Text);
                    int student_id = Convert.ToInt32(sheet.Cells[currentRow, 1].Text);
                    int score = Convert.ToInt32(sheet.Cells[currentRow, 4].Text);
                    int gradepoint;
                    if(score>=90)
                    {
                        gradepoint = 5;
                    }
                    else if(score>=80)
                    {
                        gradepoint = 4;
                    }
                    else if (score >= 70)
                    {
                        gradepoint = 3;
                    }
                    else if (score >= 60)
                    {
                        gradepoint = 2;
                    }
                    else
                    {
                        gradepoint = 0;
                    }
                    string status = sheet.Cells[currentRow, 5].Text;
                    var temp = all_students.Where(s => s.StudentId == student_id && s.Score==null).FirstOrDefault();
                    //因为不能覆盖成绩，所以已有成绩的跳过
                    if(temp.Score!=null)
                    {
                        continue;
                    }
                    if(temp!=null)
                    {
                        temp.Score = score;
                        temp.GradeStatus = status;
                        temp.InputTime = now;
                        temp.GradePoint = gradepoint;
                    }              
                }
                try
                {
                    await _context.SaveChangesAsync();
                    LogHelper.Info("Register schedule:" + schedule_id + " successfully");
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    LogHelper.Error("Database error while Register Grades ");
                    return Conflict();
                }
            }  
        }
    }
}
