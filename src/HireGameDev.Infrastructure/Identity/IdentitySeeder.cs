using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Infrastructure.Identity;

public static class IdentitySeeder
{
    private static readonly string[] Roles =
    [
        "Candidate",
        "Studio",
        "Admin"
    ];

    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in Roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Failed to create role '{role}': {errors}");
            }
        }
    }
}