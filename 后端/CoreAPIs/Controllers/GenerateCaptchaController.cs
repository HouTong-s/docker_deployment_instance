using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.Models;
using StackExchange.Redis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Timers;
using CoreAPIs.Common;

namespace CoreAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateCaptchaController : ControllerBase
    {
        // GET: api/GenerateCaptcha
        [HttpGet]
        public ActionResult<ResultModel> Get([FromQuery]string uuid)
        {
            string connect_str = SettingsReader.Getconfig()["RedisConnectionString"];
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connect_str);
            //访问redis数据库
            IDatabase db = redis.GetDatabase();  
            if(db.KeyExists(uuid))
            {
                return BadRequest();
            }
            string code = CreateRandomCode(4);
            db.StringSet(uuid, code,new TimeSpan(0,10,0));//设置10分钟的存活时间。10分钟过后就删除键值
            byte[] arr =  CreateValidGraphic(code);
            Write("./imgs/" + uuid + ".jpg", arr);
            Timer timer = new Timer();
            //设置timer，10分钟之后验证码图片自动删除
            //
            timer.Interval = 6e5;
            //timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += delegate
            {
                if (System.IO.File.Exists("./imgs/" + uuid + ".jpg"))
                    System.IO.File.Delete("./imgs/" + uuid + ".jpg");
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
            return new ResultModel { 
                code = 200,
                detail = "/imgs/" + uuid + ".jpg"
            };
        }
        private static string CreateRandomCode(int codeLen)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allCharAry = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeLen; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeLen);
                }
                temp = t;
                randomCode += allCharAry[t];
            }
            return randomCode;
        }
        private byte[] CreateValidGraphic(string validateCode)
        {
            Bitmap img = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 27);
            Graphics g = Graphics.FromImage(img);
            try
            {
                Random random = new Random();//生成随机数
                g.Clear(Color.White);//清空图片背景色
                for (int i = 0; i < 25; i++)//画图片的干扰线
                {
                    int x1 = random.Next(img.Width);
                    int x2 = random.Next(img.Width);
                    int y1 = random.Next(img.Height);
                    int y2 = random.Next(img.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, img.Width, img.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                for (int i = 0; i < 100; i++)//画图片的前景干扰点
                {
                    int x = random.Next(img.Width);
                    int y = random.Next(img.Height);
                    img.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, img.Width - 1, img.Height - 1);//画图片的边框线
                MemoryStream stream = new MemoryStream();
                img.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();//输入图片
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void Write(string path, byte[] picByte)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            //开始写入
            bw.Write(picByte, 0, picByte.Length);
            //关闭流
            bw.Close();
            fs.Close();
        }
    }
}
