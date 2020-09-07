using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure.Filters;
using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.API.Swagger;
using ToDoList.Core.Repositories;
using ToDoList.Core.Repositories.Interfaces;

namespace ToDoList.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomDbContext(Configuration, HostEnvironment);
            services.AddControllers();

            services.AddApiVersioning(Configuration);
            services.AddApiExplorer(Configuration);

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwagger(Configuration);

            services.AddScoped<ITodoListItemRepository, TodoListItemRepository>();
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();

            services.AddScoped<IFilterWrapper, FilterWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = string.Empty;
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                foreach (var description in apiProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
