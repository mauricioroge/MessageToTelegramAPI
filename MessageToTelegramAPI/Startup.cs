using MessageToTelegramAPI.BackgroundServices;
using MessageToTelegramAPI.Infra.Configurations;
using MessageToTelegramAPI.Infra.RabbitMQ;
using MessageToTelegramAPI.Infra.RabbitMQ.Interfaces;
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
            services.Configure<MainApplicationQueueConfiguration>(_configuration.GetSection("MainApplicationQueueConfiguration"));

            services.AddTransient<IRabbitMQClient, RabbitMQMainQueueClient>();
            services.AddSingleton<IRabbitMQContext, RabbitMQContext>();

            services.AddHostedService<RabbitMQConsumerBackgroundService>();
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
