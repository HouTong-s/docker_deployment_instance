using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {          
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var url = configuration["Urls"];
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /*
                     * "Urls": "http://*:8000"
                     * webBuilder.UseUrls(url);
                     * 这样就能部署
                     */
                    webBuilder.UseUrls(url);
                    webBuilder.UseStartup<Startup>();
                });
        }
            
    }
}
