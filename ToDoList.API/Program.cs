using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ToDoList.API.Extensions;

namespace ToDoList.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup(typeof(Program).Assembly.FullName))
            .Build()
            .MigrateDbContext();

            host.Run();

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}