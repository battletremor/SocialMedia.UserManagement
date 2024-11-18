using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog.Formatting.Compact;
using Serilog.Exceptions;
using SocialMedia.UserManagement.Data.Repositories;
using DTO.UserManagement.DBContexts;

namespace UserManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Configuration;
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DTOContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SMDBConnection")));
            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    //.Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers()).WriteTo.File("logs/ExceptionErrors.txt", rollingInterval: RollingInterval.Day)
                    .Enrich.WithExceptionDetails()
                    .WriteTo.File(new CompactJsonFormatter(), "logs/JsonLog.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.File("logs/UserMSLogs.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                        .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Error)
                            .WriteTo.File("logs/Errors.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"))
                            .ReadFrom.Configuration(context.Configuration);
            });
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

            //Enable CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins(allowedOrigins) // Allow only this origin (gateway)
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials()); // Allow credentials (cookies, authorization headers, etc.)
            });
            
            //JSON Serializer
            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            builder.Services
                    .AddHealthChecks()
                        .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
            
            var app = builder.Build();
            app.UseCors("AllowSpecificOrigin");

            app.MapHealthChecks("/", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }


}