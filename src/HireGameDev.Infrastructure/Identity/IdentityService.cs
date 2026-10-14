using HireGameDev.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<HireGameDev.Application.Common.IdentityResult> CreateUserAsync(
        string email,
        string password,
        string role,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return HireGameDev.Application.Common.IdentityResult.Failure(
                "A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
        {
            return HireGameDev.Application.Common.IdentityResult.Failure(
                result.Errors
                    .Select(x => x.Description)
                    .ToArray());
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return HireGameDev.Application.Common.IdentityResult.Failure(
                roleResult.Errors
                    .Select(x => x.Description)
                    .ToArray());
        }

        return HireGameDev.Application.Common.IdentityResult.Success();
    }

    public async Task<bool> UserExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);
    }
}