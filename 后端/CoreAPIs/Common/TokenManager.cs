using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using CoreAPIs.Models;

namespace CoreAPIs.Common
{
    public class TokenManager
    {
        public static string generate(int name,string role)//生成jwt码，输入的是id和身份
        {
            var claim = new Claim[]{
                    new Claim(ClaimTypes.Name,Convert.ToString(name)),
                    new Claim(ClaimTypes.Role,role)
                };
            var config = SettingsReader.Getconfig();
            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["SecretKey"]));
            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //生成token  [注意]需要nuget添加Microsoft.AspNetCore.Authentication.JwtBearer包，并引用System.IdentityModel.Tokens.Jwt命名空间
            var token = new JwtSecurityToken(config["Issuer"], config["Audience"], claim, DateTime.Now, DateTime.Now.AddMinutes(60), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static TokenVerificationResult confirm(string token)//验证jwt码
        {
            TokenVerificationResult result = new TokenVerificationResult();
            var secret = SettingsReader.GetSecretKey();
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm alg = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, alg);
                var json = decoder.Decode(token, secret, true);
                var obj = (JObject)JsonConvert.DeserializeObject(json);
                Console.WriteLine(obj);
                //校验通过，返回解密后的字符串
                result.id = Convert.ToInt32(obj[ClaimTypes.Name]);
                result.Role = (string)obj[ClaimTypes.Role];
            }
            catch (TokenExpiredException)
            {
                //表示过期
                throw;
            }
            catch (SignatureVerificationException)
            {
                //表示验证不通过
                throw;
            }
            catch (Exception)
            {
                //其他错误
                throw;
            }
            return result;
        }
    }
}
