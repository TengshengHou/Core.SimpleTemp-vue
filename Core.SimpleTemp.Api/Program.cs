using Core.SimpleTemp.Application;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

#pragma warning disable CS1591
namespace Core.SimpleTemp.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var webHost = CreateWebHostBuilder(args).Build();
            // 初始化DB
            DBInitializer.Initialize(webHost);
            CoreMapper.Initialize();
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging(
                (hostingContext, logging) =>
                {
                    //logging.ClearProviders();
                }
            )
            .UseStartup<Startup>()
            .UseNLog()
            .UseUrls("http://*:8080");
        }
    }

#pragma warning restore CS1591
}
