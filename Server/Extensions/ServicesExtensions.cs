using System.Text;
using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Trofi.io.Server.Extensions;

public static class ServicesExtensions
{
    /// <summary>
    /// This method registers the db context to the DI container as long as identity
    /// with the class AppUser. Password requirements are also configured here
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void RegisterDbContextAndIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            // configure the password requirements here...
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        }).AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    }

    /// <summary>
    /// Custom services get registered in the DI container here
    /// </summary>
    /// <param name="services"></param>
    public static void AddCustomServicesToDiContainer(this IServiceCollection services)
    {
        services.AddScoped<IFilesRepository, FilesRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
    }

    /// <summary>
    /// Adds Bearer authentcation, configures the default authentication used in the application and
    /// configures the token validation parameters used for validating the incoming JWTs
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddBearerAndConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var secret = Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!);
            o.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }
}
