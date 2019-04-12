using System;
using System.IO;
using System.Reflection;
using Gelf.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntradayDashboard.WebApi
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        public static void Main(string[] args)
        {

            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                _configuration = builder.Build();
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }            
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging((context, builder) =>
                {

                    var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddCommandLine(args)
                    .Build();
                    
                    var host = config.GetSection("Application.Greylog:Host").Value;
                    var port = Convert.ToInt32(config.GetSection("Application.Greylog:Port").Value);
                    // Console.WriteLine(host+port);

                    builder.AddConfiguration(context.Configuration.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug()
                        .AddGelf(options =>
                        {
                            // Optional customisation applied on top of settings in Logging:GELF configuration section.
                            options.Host = host;
                            options.Port = port;
                            options.LogSource = context.HostingEnvironment.ApplicationName;
                            options.AdditionalFields["machine_name"] = Environment.MachineName;
                            options.AdditionalFields["app_version"] = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                        });
                })
                .UseStartup<Startup>();
    }
}
