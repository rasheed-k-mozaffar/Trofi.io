using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Trofi.io.Server;
using Trofi.io.Server.Extensions;
using Trofi.io.Server.Options;

var builder = WebApplication.CreateBuilder(args);

#region SERVICES
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureOptions<JwtOptionsSetup>();

builder.Services.AddCustomServicesToDiContainer();

builder.Services.RegisterDbContextAndIdentity(builder.Configuration);

builder.Services.AddBearerAndConfigureAuthentication(builder.Configuration);

// This service provides access to the current authenticated user's information
// including the user's id and their cart id
builder.Services.AddScoped(sp =>
{
    var userInfo = new UserInfo();

    var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
    if (httpContextAccessor is not null)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null && httpContext.User.Identity!.IsAuthenticated)
        {
            var user = httpContext.User;
            userInfo.UserId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            userInfo.CartId = Guid.Parse(user.FindFirst("CartId")!.Value);
            userInfo.FirstName = user.FindFirst(ClaimTypes.GivenName)!.Value;
            userInfo.LastName = user.FindFirst(ClaimTypes.Surname)!.Value;
        }
    }
    return userInfo;
});

builder.Services.AddScoped(sp =>
{
    var jwtOptions = sp.GetService<IOptions<JwtOptions>>()!.Value;

    return jwtOptions;
});

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// app.UseMiddleware<JwtExpirationCheckerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
