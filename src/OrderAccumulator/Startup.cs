using Microsoft.OpenApi.Models;
using OrderAccumulator.Configuration;
using OrderAccumulator.FixHandlers;
using OrderAccumulator.Interfaces.Repositories;
using OrderAccumulator.Interfaces.Service;
using OrderAccumulator.Repositories;
using OrderAccumulator.Repositories.Context;
using OrderAccumulator.Services;
using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using System.Reflection;

namespace OrderAccumulator
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(config => config.AddConsole());
            services.AddMemoryCache();

            #region context
            services.Configure<MongoDBSettings>(
                Configuration.GetSection("MongoDBSettings"));

            services.AddSingleton<MongoDbContext>();
            #endregion

            #region repositories

            var useInMemory = Convert.ToBoolean(Configuration.GetSection("UseInMemory").Value);

            if (useInMemory)
            {
                services.AddSingleton<IExposureRepository, InMemoryExposureRepository>();
            }
            else
            {
                services.AddSingleton<IExposureRepository, ExposureRepository>();
            }
           
            #endregion

            #region Services
            services.AddSingleton<IFixMessageService, FixMessageService>();
            services.AddSingleton<IOrderService, OrderService>();
            #endregion


            #region Fix
            services.AddSingleton<IApplication, OrderAccumulatorFixApp>();
            services.AddSingleton<SessionSettings>(provider => new SessionSettings("OrderAccumulator.cfg"));
            services.AddSingleton<IMessageStoreFactory, FileStoreFactory>();
            services.AddSingleton<ILogFactory, FileLogFactory>();
            #endregion

            #region HostedService
            services.AddHostedService<FixEngineHostedService>();
            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderAccumulator API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}