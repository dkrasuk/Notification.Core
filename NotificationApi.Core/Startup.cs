using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Repository;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using Swashbuckle.AspNetCore.Swagger;

namespace NotificationApi.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddLogging();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            //Use postgresql DB
            var sqlConnectionString = Configuration.GetConnectionString("NotificationConnectionStrings");
            services.AddDbContext<NotificationContext>(option =>
            option.UseNpgsql(sqlConnectionString, b => b.MigrationsAssembly("Notification.Core")
            .MigrationsHistoryTable("__EFMigrationsHistory", "dev")
            ));

            services.AddScoped<NotificationContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //Connect SeriLog with appsetings.json   

            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json")
            //    .Build();
            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)                
            //    .WriteTo.Console()
            //    .CreateLogger();


            //Connect Log without appsetings.json
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                IndexFormat = "{0:yyyy.MM.dd}",
                TypeName = "myCustomLogEventType",
                InlineFields = true,
                MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug,
                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                       EmitEventFailureHandling.WriteToFailureSink |
                                       EmitEventFailureHandling.RaiseCallback
                // FailureSink = new FileSink("./failures.txt", new JsonFormatter(), null)
            }).CreateLogger();

            logger.AddSerilog();

            //Connect Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
