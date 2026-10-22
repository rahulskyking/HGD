using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;

namespace HireGameDev.Application.Features.StudioProfiles.UploadStudioLogo;

public sealed class UploadStudioLogoHandler(
    IStudioProfileRepository studioProfiles,
    IApplicationUnitOfWork unitOfWork,
    IFileStorage fileStorage)
{
    public async Task<OperationResult> HandleAsync(
        UploadStudioLogoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Length <= 0)
        {
            return OperationResult.Failure("Choose a logo to upload.");
        }

        if (command.Length > UploadStudioLogoCommand.MaxBytes)
        {
            return OperationResult.Failure("Logos must be 2 MB or smaller.");
        }

        var profile = await studioProfiles.GetForMediaUpdateAsync(command.UserId, cancellationToken);

        if (profile is null)
        {
            return OperationResult.Failure("Save your studio profile before adding a logo.");
        }

        var (extension, content) = await ImageSignature.DetectAsync(command.Content, cancellationToken);

        if (extension is null)
        {
            return OperationResult.Failure("Logos must be JPEG, PNG or WebP images.");
        }

        var previousUrl = profile.LogoUrl;
        var url = await fileStorage.SaveAsync(content, UploadStudioLogoCommand.Folder, extension, cancellationToken);

        profile.SetLogo(url);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousUrl is not null)
        {
            await fileStorage.DeleteAsync(previousUrl, cancellationToken);
        }

        return OperationResult.Success();
    }
}
