using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using ToDoList.API.Infrastructure;

namespace ToDoList.API.Extensions
{
    public static class HosExtensionMethods
    {
        public static IHost MigrateDbContext(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ToDoList.Core.Context.TodoListContext>();

                try
                {
                    new TodoListContextSeed().SeedAsync(context).Wait();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return host;
        }
    }
}