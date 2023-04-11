using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoreAPIs
{
    using DbModels;
    using System.IO;
    using MiddleWare;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.AspNetCore.Http;
    using CoreAPIs.Filters;
    using CoreAPIs.Common;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogHelper.Configure();//ע��log4net
        }
        ~Startup()
        {
            _logStream.Dispose();
        }
        public IConfiguration Configuration { get; }
        private readonly StreamWriter _logStream = new StreamWriter("DBLog.txt", append: true);
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));//ȫ��ע���쳣������
            }).AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreAPIs", Version = "v1" });
            });
            //�������ķ���
            //���cors ���� ���ÿ�����   

            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                    //.AllowCredentials()//ָ������cookie
                .AllowAnyOrigin(); //�����κ���Դ����������
                });
            });
            var connectionString = SettingsReader.GetMySQLConnectionString();//��ȡjson�ļ�
            // Replace with your server version and type.
            // Use 'MariaDbServerVersion' for MariaDB.
            // Alternatively, use 'ServerVersion.AutoDetect(connectionString)'.
            // For common usages, see pull request #1233.
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 28));
            // Replace 'YourDbContext' with the name of your own DbContext derived class.

            services.AddDbContext<schoolContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion
                    
                    )
                    // The following three options help with debugging, but should
                    // be changed or removed for production.
                    //Microsoft.Extensions 
                    .LogTo(_logStream.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )
        {
            app.UseMiddleware<TokenMiddleware>();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreAPIs v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("any");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
