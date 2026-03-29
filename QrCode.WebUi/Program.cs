using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QrCode.Application;
using QrCode.Domain.AggregateModels.Email;
using QrCode.Domain.SeedWork;
using QrCode.Infrastructure;
using QrCode.Infrastructure.Context;
using QrCode.WebUi.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Configuration;
using System.Net;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    QrCode.Infrastructure.DependencyInjection.Configure(options, serviceProvider.GetRequiredService<IConfiguration>());
});

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information() // Minimum log seviyesi
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Microsoft loglarý için minimum seviye
        .Enrich.FromLogContext()
        .Enrich.With(new PageActionEnricher())
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("wwwroot\\Logs\\log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}]  {Message:lj}{NewLine}")
        .WriteTo.Console(); // Konsola yazma
        
});

//builder.Services.AddMediatR(typeof(Program));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication().AddGoogle(opt =>
{
    opt.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    opt.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.MapGet("/user/login", context =>
//{
//    if (!context.User.Identity.IsAuthenticated)
//    {
//        // Kullanýcý yetkilendirilmiþ deðilse, oturum açma sayfasýna yönlendir
//        return Task.FromResult((IActionResult)new RedirectToPageResult("/user/login"));
//    }

//    // Kullanýcý yetkilendirilmiþse, NotFound döndür
//    return Task.FromResult((IActionResult)new NotFoundResult());
//});
//app.Use(async (context, next) =>
//{
//    if (!context.User.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/User") && !context.Request.Path.ToString().Contains("Create"))
//    {

//        context.Response.Redirect("/User/Login");
//        return;
//    }

//    await next();
//});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
