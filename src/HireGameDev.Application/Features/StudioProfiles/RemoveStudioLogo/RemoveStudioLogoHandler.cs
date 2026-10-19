using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.StudioProfiles.RemoveStudioLogo;

public sealed class RemoveStudioLogoHandler(
    IApplicationDbContext dbContext,
    IFileStorage fileStorage)
{
    public async Task HandleAsync(
        RemoveStudioLogoCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.StudioProfiles
            .SingleOrDefaultAsync(x => x.UserId == command.UserId, cancellationToken);

        if (profile?.LogoUrl is null)
        {
            return;
        }

        var url = profile.LogoUrl;

        profile.RemoveLogo();
        await dbContext.SaveChangesAsync(cancellationToken);
        await fileStorage.DeleteAsync(url, cancellationToken);
    }
}
