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
using System.Security.Cryptography;

namespace CoreAPIs.Controllers.StudentControllers
{
    [Route("api/students/[controller]")]
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
        *@TODO:学生登录
        *@param {int} id 学生id
        *@param {string} password 密码
        *@param {string} uuid 验证码id
        *@param {string} captcha 验证码的值
        *
        *@return 
        *成功或失败都返回状态码
        *
        */
        public async Task<ActionResult<TokenStatus>> LoginAsync([FromHeader] int id, [FromHeader] string password,dynamic q)
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
            if(db.StringGet(uuid).ToString() != captcha.ToUpper())
            {
                return BadRequest();
            }
            //以上都是验证码错误的情形(返回值400)
            //验证码正确时就把图片删除了。redis数据库的值也删除(当然也可以不删除)
            if(System.IO.File.Exists("../imgs/" + uuid + ".jpg"))
                System.IO.File.Delete("../imgs/"+uuid+".jpg");
            db.KeyDelete(uuid);
            int student_id = id;
            string _password = password;
            
            var _student = await _context.Students.Where(s => s.StudentId == student_id).Select(s => s).FirstOrDefaultAsync();

            if (_student != null)
            {
                //学生已毕业
                if(_student.IsGraduate == 1)
                {
                    return NotFound();
                }
                string salt = _student.Salt;
                if (HashHelper.CallMD5(salt[0] + _password + salt[1..]) == _student.Password)
                {
                    LogHelper.Info("Student:" + student_id + "has already logged in");
                    return new TokenStatus(TokenManager.generate(student_id, "student"), 1);
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
