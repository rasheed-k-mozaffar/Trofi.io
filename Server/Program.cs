using Microsoft.AspNetCore.ResponseCompression;
using Trofi.io.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region SERVICES
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddCustomServicesToDiContainer();

builder.Services.RegisterDbContextAndIdentity(builder.Configuration);
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

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
