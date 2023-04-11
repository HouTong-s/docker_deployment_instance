using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPIs.DbModels;
using CoreAPIs.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Text;
using CoreAPIs.Common;
using StackExchange.Redis;

namespace CoreAPIs.Controllers.TeacherControllers
{
    [Route("api/teachers/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly schoolContext _context;
        public LoginController(schoolContext context)
        {
            _context = context;
        }

        [HttpPost]
        /*
        *@TODO:老师登录
        *@param {int} id 老师id
        *@param {string} password 密码
        *@param {string} uuid 验证码id
        *@param {string} captcha 验证码的值
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult<TokenStatus>> LoginAsync([FromHeader] int id, [FromHeader] string password, dynamic q)
        {
            string uuid = Convert.ToString(q.uuid);
            string captcha = Convert.ToString(q.captcha);
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
            //验证码正确时就把图片删除了,redis数据库的值也删除了(当然也可以不删除)
            if (System.IO.File.Exists("../imgs/" + uuid + ".jpg"))
                System.IO.File.Delete("../imgs/" + uuid + ".jpg");
            db.KeyDelete(uuid);
            int taecher_id = id;
            string _password = password;
            var _teacher = await _context.Teachers.Where(s => s.TeacherId == taecher_id).Select(s => s).FirstOrDefaultAsync();

            if (_teacher != null)
            {
                //老师已离职
                if(_teacher.IsQuit == 1)
                {
                    return NotFound();
                }
                string salt = _teacher.Salt;
                if (HashHelper.CallMD5(salt[0] + _password + salt[1..]) == _teacher.Password)
                {
                    LogHelper.Info("Teacher:" + taecher_id + " has already logged in");
                    return new TokenStatus(TokenManager.generate(taecher_id, "teacher"), 1);
                }
                else
                {
                    LogHelper.Error("Incorrect username or password");
                    return NotFound();
                }
                
            }
            else
            {
                LogHelper.Error("Incorrect username or password");
                return NotFound();
            }
        }


    }
}
