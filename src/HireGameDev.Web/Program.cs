using HireGameDev.Application;
using HireGameDev.Infrastructure;
using HireGameDev.Infrastructure.Identity;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthenticationProviders(builder.Configuration, builder.Environment);
builder.Services.AddRolePolicies();

builder.Services.AddRazorPages(options =>
{
    // Every page in these folders requires the matching role; new pages are protected by default.
    options.Conventions.AuthorizeFolder("/Candidate", RolePolicies.Candidate);
    options.Conventions.AuthorizeFolder("/Studio", RolePolicies.Studio);
});

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
