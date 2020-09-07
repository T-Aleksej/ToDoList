using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ToDoList.API.Swagger;
using ToDoList.Core.Context;

namespace ToDoList.API.Extensions
{
    public static class ServiceExtensionMethods
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                // Prod, windows and linux
                services.AddDbContext<TodoListContext>(options =>
                {
                    options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("ToDoList.Data");
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                });
            }
            else
            {
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                if (isWindows)
                {
                    // Dev windows
                    services.AddDbContext<TodoListContext>(options =>
                    {
                        options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
                    });
                }
                else
                {
                     // Dev linux
                    services.AddDbContext<TodoListContext>(options =>
                    {
                        options.UseSqlite(configuration["ConnectionStrings:SQLiteConnection"]);
                    });
                }
            }

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name }.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        public static IServiceCollection AddApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true; // api-supported-versions api-deprecated-versions
            });

            return services;
        }

        public static IServiceCollection AddApiExplorer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}