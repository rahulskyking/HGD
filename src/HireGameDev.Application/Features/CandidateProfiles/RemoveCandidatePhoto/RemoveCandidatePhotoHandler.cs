using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;

namespace HireGameDev.Application.Features.CandidateProfiles.RemoveCandidatePhoto;

public sealed class RemoveCandidatePhotoHandler(
    ICandidateProfileRepository candidateProfiles,
    IApplicationUnitOfWork unitOfWork,
    IFileStorage fileStorage)
{
    public async Task HandleAsync(
        RemoveCandidatePhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await candidateProfiles.GetForMediaUpdateAsync(command.UserId, cancellationToken);

        if (profile?.PhotoUrl is null)
        {
            return;
        }

        var url = profile.PhotoUrl;

        profile.RemovePhoto();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await fileStorage.DeleteAsync(url, cancellationToken);
    }
}
