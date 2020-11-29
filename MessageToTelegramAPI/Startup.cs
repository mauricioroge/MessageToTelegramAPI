using MessageToTelegramAPI.BackgroundServices;
using MessageToTelegramAPI.Domain.Services;
using MessageToTelegramAPI.Infra.Configurations;
using MessageToTelegramAPI.Infra.RabbitMQ;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
using MessageToTelegramAPI.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageToTelegramAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RabbitMQConfiguration>(_configuration.GetSection("RabbitMQConfiguration"));
            services.Configure<UserMessagesQueueConfiguration>(_configuration.GetSection("UserMessagesQueueConfiguration"));
            services.Configure<ServerMessagesQueueConfiguration>(_configuration.GetSection("ServerMessagesQueueConfiguration"));
            services.Configure<TelegramBotConfiguration>(_configuration.GetSection("TelegramBotConfiguration"));

            services.AddTransient<IServerToUserMQClient, ServerToUserMQClient>();
            services.AddTransient<IUserToServerMQClient, UserToServerMQClient>();
            services.AddSingleton<IRabbitMQContext, RabbitMQContext>();

            
            services.AddTransient<ITelegramService, TelegramService>();
            
            
            services.AddHostedService<MessageConsumerFromServerBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();
            
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
