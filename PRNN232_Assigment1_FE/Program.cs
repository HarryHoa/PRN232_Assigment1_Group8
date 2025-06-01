using DAL.Models;
using Microsoft.EntityFrameworkCore;
using PRNN232_Assigment1_FE.Controllers;
using System.Configuration;

namespace PRNN232_Assigment1_FE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});

            // Fix for CS1929: Use IHttpClientBuilder returned by AddHttpClient() to configure the primary HTTP message handler.
            builder.Services.AddDbContext<FUNewsManagementContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient<LoginController>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");
                client.Timeout = TimeSpan.FromSeconds(30);
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
            //app.UseSession(); // Add this line before app.UseAuthorization();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
