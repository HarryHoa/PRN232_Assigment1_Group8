
using Common.Dto;
using Common.Validator;
using DAL.Models;
using DAL.Repository;
using DAL.Repository.Impl;
using DLL;
using DLL.Interface;
using DLL.Mapping;
using DLL.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;

namespace PRN232_ASSI1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Category>("Categories");
            modelBuilder.EntitySet<AdminCRUDdto>("AdminCrudAccount");
            modelBuilder.EntitySet<NewsArticle>("NewsArticles");
            modelBuilder.EntitySet<NewsArticle>("NewsArticleOdata"); 
            
            // Add services to the container.
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
             options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
             options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            })
            .AddOData(
             options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
            "odata",
            modelBuilder.GetEdmModel()));


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
            builder.Services.AddScoped<INewArticleService, NewArticleService>();
            builder.Services.AddScoped<IAdminCrudAccountService, AdminCrudAccountService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            //builder.Services.AddScoped<SystemAccountService>();
            builder.Services.AddScoped<INewsArticleService, NewsArticleService>();


            builder.Services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SystemAccountDtoValidator>());

            builder.Services.AddAutoMapper(typeof(NewsArticleMappingProfile));
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
