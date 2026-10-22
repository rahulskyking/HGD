using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;

namespace HireGameDev.Application.Features.StudioProfiles.RemoveStudioLogo;

public sealed class RemoveStudioLogoHandler(
    IStudioProfileRepository studioProfiles,
    IApplicationUnitOfWork unitOfWork,
    IFileStorage fileStorage)
{
    public async Task HandleAsync(
        RemoveStudioLogoCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await studioProfiles.GetForMediaUpdateAsync(command.UserId, cancellationToken);

        if (profile?.LogoUrl is null)
        {
            return;
        }

        var url = profile.LogoUrl;

        profile.RemoveLogo();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await fileStorage.DeleteAsync(url, cancellationToken);
    }
}
