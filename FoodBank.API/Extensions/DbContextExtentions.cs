using CaptionGenerator.EF.Data;
using Microsoft.EntityFrameworkCore;

namespace CaptionGenerator.API.Extensions
{
    public static class DbContextExtensions
    {
        public static void AddDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SQLiteConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }
    }
}