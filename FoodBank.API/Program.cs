
using CaptionGenerator.API.Extensions;
using CaptionGenerator.EF.Data;
using FoodBank.API.Extensions;
using FoodBank.CORE.Entities;
using FoodBank.EF.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace FoodBank.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configuration
            var Configuration = builder.Configuration;

            // Configure Cloudinary settings
            builder.Services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));


        //    builder.Services.AddControllersWithViews()
        //.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        //});

            builder.Services.AddCustomServices();
            builder.Services.AddDbContextExtension(Configuration);
            builder.Services.AddJwtAuthentication(Configuration);
            builder.Services.AddSwaggerGenExtension();
            // Add Identity to the project
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
