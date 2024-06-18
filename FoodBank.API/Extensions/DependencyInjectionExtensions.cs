using CaptionGenerator.EF.Repositories;
using FoodBank.CORE.Interfaces;
using FoodBank.EF.Implementions;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBank.API.Extensions
{

        public static class DependencyInjectionExtensions
        {
            public static void AddCustomServices(this IServiceCollection services)
            {
                services.AddScoped<IAuthUser, AuthUser>();
                services.AddScoped<IPhotoService, PhotoService>();
            }
        }
    
}
