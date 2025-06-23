
using Common.Dto;
using Common.SettingJWT;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using PRN232_ASSI1.MiddleWare;
using System.Text;
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
            builder.Services.AddSwaggerGen(c =>
            {


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter your JWT token without 'Bearer' prefix.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",


                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SystemAccountDtoValidator>());
            var jwtKey = Encoding.ASCII.GetBytes(JwtSettingModel.SecretKey);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    ValidateIssuer = true,
                    ValidIssuer = JwtSettingModel.Issuer,
                    ValidateAudience = true,
                    ValidAudience = JwtSettingModel.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // không delay thêm thời gian token hết hạn
                };
            });

            // ------------------ Authorization ------------------
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("3"));
                options.AddPolicy("LecturerOnly", policy => policy.RequireRole("2"));
                options.AddPolicy("StaffOnly", policy => policy.RequireRole("1"));
            });


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
            app.UseMiddleware<CustomJwtMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
