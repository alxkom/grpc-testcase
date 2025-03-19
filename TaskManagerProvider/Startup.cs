using TaskManagerProvider.Services;
using TaskManagerProvider.Domain.Repositories;
using TaskManagerProvider.Storage.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManager.Models;
using TaskManagerProvider.Domain.Engines;
using Task = TaskManager.Models.Task;

namespace TaskManagerProvider
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 2 * 1024 * 1024; // 2 MB
                options.MaxSendMessageSize = 5 * 1024 * 1024; // 5 MB
            });

            services.AddAutoMapper(typeof(AppMappingProfile));

            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskManagerEngine, TaskManagerEngine>();

            services.AddLogging();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient(
                    typeof(IDefaultDataProvider<User>),
                    typeof(UsersDefaultDataProvider)));

            services.TryAddEnumerable(
                ServiceDescriptor.Transient(
                    typeof(IDefaultDataProvider<Task>),
                    typeof(TasksDefaultDataProvider)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TaskManagerService>();

                endpoints.MapGet("/", async context => 
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoint must be maid through a gRPC client.");
                });
            });
        }
    }
}