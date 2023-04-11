using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;
using OfficeOpenXml;
using System.IO;
using CoreAPIs.Common;
using System.Timers;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class ExportStudentsController : ControllerBase
    {
        private readonly schoolContext _context;
        private int rep = 0;
        public ExportStudentsController(schoolContext context)
        {
            _context = context;
        }
        // GET: api/teachers/ExportStudents
        [HttpGet]
        /*
         *@TODO:关于某节排课的学生信息生成excel文件
	     *@param {int} schedule_id 排课号
	     *
	     *@return 
	     *成功返回文件地址
	     *失败返回404
         */
        public async Task<ActionResult<ResultModel>> GetStudents([FromQuery] int schedule_id)
        {
            var results = await (from en in
                                       (from sch in
                                            (from schedule in _context.Schedules
                                             where schedule.ScheduleId == schedule_id
                                             select schedule)
                                        join enrollment in _context.Enrollments
                                        on sch.ScheduleId equals enrollment.ScheduleId
                                        select enrollment)
                                 orderby en.StudentId
                                 join student in _context.Students
                                 on en.StudentId equals student.StudentId
                                 select new Student_basic_info
                                 {
                                     StudentId = en.StudentId,
                                     score = en.Score,
                                     GradeStatus = en.GradeStatus,
                                     StudentName = student.StudentName,
                                     Department = student.Department
                                 }
                                   ).ToListAsync();
            Console.WriteLine(results.Count);
            if (results.Count > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string code = GenerateCheckCodeNum(10);
                var filename = "./export/" + code + ".xlsx";
                var package = new ExcelPackage(new FileInfo(filename));
                var worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells[1, 1].Value = "学号";
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "专业";
                worksheet.Cells[1, 4].Value = "分数";
                worksheet.Cells[1, 5].Value = "成绩状态";
                for (int i=0;i<results.Count;i++)
                {
                    worksheet.Cells[2 + i, 1].Value = results[i].StudentId;
                    worksheet.Cells[2 + i, 2].Value = results[i].StudentName;
                    worksheet.Cells[2 + i, 3].Value = results[i].Department;
                    worksheet.Cells[2 + i, 4].Value = results[i].score;
                    worksheet.Cells[2 + i, 5].Value = results[i].GradeStatus;
                }
                
                await package.SaveAsync();
                Timer timer = new Timer();
                //设置timer，1分钟之后导出文件自动删除
                //
                timer.Interval = 6e4;
                //timer.AutoReset = false;
                timer.Enabled = true;
                timer.Elapsed += delegate
                {
                    if (System.IO.File.Exists("./export/" + code + ".xlsx"))
                        System.IO.File.Delete("./export/" + code + ".xlsx");
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
                LogHelper.Info("Export " + schedule_id  + " lesson successfully.");
                return new ResultModel
                {
                    code = 200,
                    detail = "/export/" + code + ".xlsx"
                };
            }
            else
            {
                return NotFound();
            }
        }
        //产生随机数
        private string GenerateCheckCodeNum(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }
    }
}
