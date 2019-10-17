using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((buildercontext, config) =>
            {
                var env = buildercontext.HostingEnvironment;
                config.SetBasePath(env.ContentRootPath);
                config.AddJsonFile("appsettings." + env.EnvironmentName + ".json", optional: false, reloadOnChange: true);
                Environment.SetEnvironmentVariable("SettingFileDate", new FileInfo("appsettings." + env.EnvironmentName + ".json").LastWriteTime.ToString("s"));
                config.AddEnvironmentVariables();

            })
                .UseStartup<Startup>();
    }
}
