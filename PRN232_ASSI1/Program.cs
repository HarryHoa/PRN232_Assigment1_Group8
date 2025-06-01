
using Common.Validator;
using DAL.Models;
using DAL.Repository;
using DAL.Repository.Impl;
using DLL;
using DLL.Interface;
using DLL.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace PRN232_ASSI1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
            builder.Services.AddScoped<INewArticleService, NewArticleService>();
            builder.Services.AddScoped<IAdminCrudAccountService, AdminCrudAccountService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddAutoMapper(typeof(MappingProfile)); // Add your profile here
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            //builder.Services.AddScoped<SystemAccountService>();


            builder.Services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SystemAccountDtoValidator>());


            // Updated to use the recommended method for registering validators


            builder.Services.AddDbContext<FUNewsManagementContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
