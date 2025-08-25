using Microsoft.OpenApi.Models;
using OrderGenerator.Configuration;
using OrderGenerator.FixHandlers;
using OrderGenerator.HostedServices;
using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Interfaces.Services;
using OrderGenerator.Repositories;
using OrderGenerator.Repositories.Context;
using OrderGenerator.Services;
using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using System.Reflection;
using FluentValidation;
using OrderGenerator.Validators;
using FluentValidation.AspNetCore;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        #region Validator
        services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        #endregion

        services.AddLogging(config => config.AddConsole());

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderGenerator API", Version = "v1" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
            else
            {
                Console.WriteLine($"Atenção: Arquivo de comentários XML não encontrado em {xmlPath}. Documentação será limitada.");
            }
        });



        #region context
        services.Configure<MongoDBSettings>(
            Configuration.GetSection("MongoDBSettings"));

        services.AddSingleton<MongoDbContext>();
        #endregion




        #region repositories
        var useInMemory = Convert.ToBoolean(Configuration.GetSection("UseInMemory").Value);

        if (useInMemory)
        {
            services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            services.AddSingleton<IAssetRepository, InMemoryAssetRepository>();
        }
        else
        {
            services.AddSingleton<IAssetRepository, AssetRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
        }
        #endregion

        #region services
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<IAssetService, AssetService>();

        #endregion

        #region FIX
        services.AddSingleton<IFixMessageService, FixMessageService>();

        services.AddSingleton<IApplication, OrderGeneratorFixApp>();
        services.AddSingleton(provider => new SessionSettings("OrderGenerator.cfg"));
        services.AddSingleton<IMessageStoreFactory, FileStoreFactory>();
        services.AddSingleton<ILogFactory, FileLogFactory>();
        #endregion

        #region hostedservice
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderGenerator API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}