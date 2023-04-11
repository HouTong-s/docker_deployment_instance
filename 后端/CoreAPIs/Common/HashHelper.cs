using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CoreAPIs.Common
{
    public class HashHelper
    {
        //生成MD5码
        public static string CallMD5(string str)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return CallMD5(bytes);
        }

        public static string CallMD5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(bytes);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("X").Length == 1 ? "0" + computeBytes[i].ToString("X") : computeBytes[i].ToString("X");
                }
                return result;
            }
        }

        public static string CallMD5(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(stream);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("X").Length == 1 ? "0" + computeBytes[i].ToString("X") : computeBytes[i].ToString("X");
                }
                return result;
            }
        }

        //生成盐
        public static string CreateVerifyCode(int len)
        {
            char[] data = { 'a','c','d','e','f','h','k','m',
                'n','r','s','t','w','x','y'};
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < len; i++)
            {
                int index = rand.Next(data.Length);//[0,data.length)
                char ch = data[index];
                sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
