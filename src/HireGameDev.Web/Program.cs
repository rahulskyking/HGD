using HireGameDev.Application;
using HireGameDev.Infrastructure;
using HireGameDev.Infrastructure.Identity;
using HireGameDev.Infrastructure.Persistence;
using HireGameDev.Infrastructure.Storage;
using HireGameDev.Web;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// One registration method per layer; each project's DependencyInjection.cs lists what it owns.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWeb(builder.Configuration, builder.Environment);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
    await SkillSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Uploads live outside wwwroot (App_Data/media by default) and are served read-only under /media.
var fileStorage = app.Services.GetRequiredService<IOptions<FileStorageSettings>>().Value;
var uploadsRoot = fileStorage.ResolveRootPath(app.Environment.ContentRootPath);
Directory.CreateDirectory(uploadsRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = fileStorage.RequestPath,
    ServeUnknownFileTypes = false,
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Explicit routes keep the role-based URLs readable while controllers and views own the UI.
app.MapControllerRoute(
    name: "candidate-profile",
    pattern: "Candidate/Profile/{action=Index}",
    defaults: new { controller = "CandidateProfile" });
app.MapControllerRoute(
    name: "studio-profile",
    pattern: "Studio/Profile/{action=Index}",
    defaults: new { controller = "StudioProfile" });
app.MapControllerRoute(
    name: "studio-jobs",
    pattern: "Studio/Jobs/{action=Index}/{id?}",
    defaults: new { controller = "StudioJobs" });
app.MapControllerRoute(
    name: "privacy",
    pattern: "Privacy",
    defaults: new { controller = "Home", action = "Privacy" });
app.MapControllerRoute(
    name: "styleguide",
    pattern: "Styleguide",
    defaults: new { controller = "Home", action = "Styleguide" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
