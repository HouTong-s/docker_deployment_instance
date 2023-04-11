using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using OfficeOpenXml;

namespace CoreAPIs.Controllers.AdminControllers.ScheduleManagement
{
    [Route("api/admins/[controller]")]
    [ApiController]
    public class AddSchedulesByFileController : ControllerBase
    {
        private readonly schoolContext _context;

        public AddSchedulesByFileController(schoolContext context)
        {
            _context = context;
        }

        // POST: api/admins/AddSchedulesByFile
        [HttpPost]
        /*
        *@TODO:新增一堆排课信息
        *@param {IFormFile} file 文件
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult> RegisterSchedulesByFile([FromForm] IFormFile file )
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
                        int LessonId = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        i++;
                        int TeacherId = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        i++;
                        int Year = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        int Half = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        int BeginWeek = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        int EndWeek = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        string Place = sheet.Cells[currentRow, i++].Text;
                        int MaxNum = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        string Note = sheet.Cells[currentRow, i++].Text;
                        string TeachingMaterial = sheet.Cells[currentRow, i++].Text;
                        int CanRetake = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                        string Campus = sheet.Cells[currentRow, i++].Text;
                        Schedule new_schedule = new Schedule
                        {
                            LessonId = LessonId,
                            TeacherId = TeacherId,
                            Year = Year,
                            Half = Half,
                            BeginWeek = BeginWeek,
                            EndWeek = EndWeek,
                            Place = Place,
                            MaxNum = MaxNum,
                            Note = Note,
                            TeachingMaterial = TeachingMaterial,
                            CanRetake = CanRetake,
                            Campus = Campus,
                        };
                        _context.Schedules.Add(new_schedule);
                        await _context.SaveChangesAsync();
                        int schedule_id = new_schedule.ScheduleId;
                        while (sheet.Cells[currentRow, i].Text != "")
                        {
                            int DayWeek = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            int BeginSection = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            int EndSection = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            int SingleOrDouble = Convert.ToInt32(sheet.Cells[currentRow, i++].Text);
                            _context.ScheduleTimes.Add(new ScheduleTime
                            {
                                ScheduleId = schedule_id,
                                DayWeek = DayWeek,
                                BeginSection = BeginSection,
                                EndSection = EndSection,
                                SingleOrDouble = SingleOrDouble
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
