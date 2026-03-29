using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using QrCode.Application.Features.Users.CreateUser;
using QrCode.Domain.AggregateModels.Email;
using QrCode.Domain.AggregateModels.Email.Events;
using QrCode.Domain.AggregateModels.Tags;
using QrCode.Domain.AggregateModels.Tags.Events;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.AggregateModels.Users.Events;
using QrCode.Domain.SeedWork;
using QrCode.Domain.Validator;
using QrCode.Infrastructure.Context;
using QrCode.Infrastructure.Service;
using Serilog;
using Serilog.Events;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System;
using System.Configuration;
using System.Net;
using System.Diagnostics;

namespace QrCode.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            try
            {
                services.AddScoped<ApplicationDbContext>();
                services.AddScoped<IUnitOfWork>(opt => opt.GetRequiredService<ApplicationDbContext>());
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<ITagService, TagService>();
                services.AddScoped<IEmailService, EmailService>();

                //services.AddScoped<INotificationHandler<EmailSentEvent>, EmailSentEventHandler>();
                services.Configure<IdentityOptions>(options =>
                {
                    // Şifre gereksinimlerini yapılandırma
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 3; // Şifrenin en az 3 karakter olması gerekiyor
                });
                services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
                {
                    opt.SignIn.RequireConfirmedEmail = true;
                    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                services.AddScoped<IValidator<CreateUserCommand>, UserCreateValidator>();
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                //Serilog'un yapılandırılması
                //var pageActionEnricher = new PageActionEnricher();

                //Log.Logger = new LoggerConfiguration()
                //    .MinimumLevel.Debug()
                //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                //    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                //    .MinimumLevel.Information()
                //    .Enrich.FromLogContext()
                //    .Enrich.With(pageActionEnricher)
                //    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                //    .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                //    .CreateLogger();
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return services;
        }
        //public static ILogger CreateLogger()
        //{

        //    //var pageActionEnricher = new PageActionEnricher();
        //    var logger = new LoggerConfiguration()
        //        .MinimumLevel.Debug()
        //        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        //        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        //        .MinimumLevel.Information()
        //        .Enrich.FromLogContext()
        //        .Enrich.With(new PageActionEnricher()) // Doğru kullanım
        //        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Page}.{Action} ({LineNumber}): {Message:lj}{NewLine}{Exception}")
        //        .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} {Page}.{Action} ({LineNumber}): {Message:lj}{NewLine}{Exception}")
        //        .CreateLogger();
        //    Log.Logger = logger;

        //    return logger;

        //}
        public static LogEvent EnrichWithPageAndAction(LogEvent logEvent)
        {
            var frame = new StackFrame(1, true);
            var method = frame.GetMethod();

            logEvent.AddPropertyIfAbsent(new LogEventProperty("Page", new ScalarValue(method.DeclaringType?.Name)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("Action", new ScalarValue(method.Name)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("LineNumber", new ScalarValue(frame.GetFileLineNumber())));

            return logEvent;
        }
        public static void Configure(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }


    }
}
