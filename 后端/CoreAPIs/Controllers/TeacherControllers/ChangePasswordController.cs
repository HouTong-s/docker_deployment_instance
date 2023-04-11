using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using StackExchange.Redis;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class ChangePasswordController : ControllerBase
    {
        private readonly schoolContext _context;

        public ChangePasswordController(schoolContext context)
        {
            _context = context;
        }
        // POST: api/teachers/ChangePassword
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        /*
         *@TODO:改密码
	     *@param {string} old_password 原来的密码
	     *@param {string} new_password 新密码
	     *@param {string} uuid 验证码id
	     *@param {string} captcha 验证码的值
	     *
	     *@return 
	     *成功或失败都返回状态码
	     *
         * */
        public async Task<ActionResult<Teacher>> PostTeacher(dynamic q)
        {
            string old_password = Convert.ToString(q.old_password);
            string new_password = Convert.ToString(q.new_password);
            string uuid = Convert.ToString(q.uuid);
            string captcha = Convert.ToString(q.captcha);
            var teacher_id = Convert.ToInt32(Request.Headers["ids"]);
            Console.WriteLine(uuid);
            Console.WriteLine(captcha);
            string connect_str = SettingsReader.Getconfig()["RedisConnectionString"];
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connect_str);

            //访问redis数据库
            IDatabase db = redis.GetDatabase();
            if (!db.KeyExists(uuid))
            {
                return BadRequest();
            }
            if (db.StringGet(uuid).ToString() != captcha.ToUpper())
            {
                return BadRequest();
            }
            //以上都是验证码错误的情形(返回值400)
            //验证码正确时就把图片删除了,redis数据库的值也删除了
            if (System.IO.File.Exists("../imgs/" + uuid + ".jpg"))
                System.IO.File.Delete("../imgs/" + uuid + ".jpg");
            db.KeyDelete(uuid);
            var _teacher = await _context.Teachers.Where(s => s.TeacherId == teacher_id).Select(s => s).FirstOrDefaultAsync();

            if (_teacher != null && HashHelper.CallMD5(_teacher.Salt[0] + old_password + _teacher.Salt[1..]) == _teacher.Password)
            {
                _teacher.Password = HashHelper.CallMD5(_teacher.Salt[0] + new_password + _teacher.Salt[1..]);
                await _context.SaveChangesAsync();
                try
                {
                    LogHelper.Info("Teacher:" + teacher_id + "has already changed password");
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception)
                {
                    LogHelper.Error("Database Exception");
                    return NotFound();
                }
            }
            else
            {
                LogHelper.Error("Incorrect password");
                return NotFound();
            }
        }
    }
}
