using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;

public sealed class UploadCandidatePhotoHandler(
    IApplicationDbContext dbContext,
    IFileStorage fileStorage)
{
    public async Task<OperationResult> HandleAsync(
        UploadCandidatePhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Length <= 0)
        {
            return OperationResult.Failure("Choose a photo to upload.");
        }

        if (command.Length > UploadCandidatePhotoCommand.MaxBytes)
        {
            return OperationResult.Failure("Photos must be 2 MB or smaller.");
        }

        var profile = await dbContext.CandidateProfiles
            .SingleOrDefaultAsync(x => x.UserId == command.UserId, cancellationToken);

        if (profile is null)
        {
            return OperationResult.Failure("Save your profile before adding a photo.");
        }

        var (extension, content) = await ImageSignature.DetectAsync(command.Content, cancellationToken);

        if (extension is null)
        {
            return OperationResult.Failure("Photos must be JPEG, PNG or WebP images.");
        }

        var previousUrl = profile.PhotoUrl;
        var url = await fileStorage.SaveAsync(content, UploadCandidatePhotoCommand.Folder, extension, cancellationToken);

        profile.SetPhoto(url);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (previousUrl is not null)
        {
            await fileStorage.DeleteAsync(previousUrl, cancellationToken);
        }

        return OperationResult.Success();
    }
}
