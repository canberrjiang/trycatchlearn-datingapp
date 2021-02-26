using API.Data;
using API.Helper;
using API.Interface;
using API.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
      services.AddDbContext<DataContext>(options =>
      {
        options.UseSqlite(config.GetConnectionString("DefaultConnection"));
      });

      return services;
    }
  }
}