using HireGameDev.Application;
using HireGameDev.Infrastructure;
using HireGameDev.Infrastructure.Identity;
using HireGameDev.Web;

var builder = WebApplication.CreateBuilder(args);

// One registration method per layer; each project's DependencyInjection.cs lists what it owns.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWeb(builder.Configuration, builder.Environment);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
