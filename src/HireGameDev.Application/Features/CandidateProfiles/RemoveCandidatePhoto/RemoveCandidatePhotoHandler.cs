using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.CandidateProfiles.RemoveCandidatePhoto;

public sealed class RemoveCandidatePhotoHandler(
    IApplicationDbContext dbContext,
    IFileStorage fileStorage)
{
    public async Task HandleAsync(
        RemoveCandidatePhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.CandidateProfiles
            .SingleOrDefaultAsync(x => x.UserId == command.UserId, cancellationToken);

        if (profile?.PhotoUrl is null)
        {
            return;
        }

        var url = profile.PhotoUrl;

        profile.RemovePhoto();
        await dbContext.SaveChangesAsync(cancellationToken);
        await fileStorage.DeleteAsync(url, cancellationToken);
    }
}
