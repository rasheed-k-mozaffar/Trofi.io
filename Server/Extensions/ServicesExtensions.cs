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
        // ex: services.AddScoped<ICustomService, CustomService>();
    }
}
