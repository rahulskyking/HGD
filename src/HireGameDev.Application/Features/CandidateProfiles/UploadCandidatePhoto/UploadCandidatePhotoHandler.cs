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

        var (extension, content) = await InspectAsync(command.Content, cancellationToken);

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

    /// <summary>
    /// Reads the leading bytes to identify the format, then hands back a stream rewound to the
    /// start so storage receives the whole file. Non-seekable input is buffered first.
    /// </summary>
    private static async Task<(string? Extension, Stream Content)> InspectAsync(
        Stream content,
        CancellationToken cancellationToken)
    {
        if (!content.CanSeek)
        {
            var buffered = new MemoryStream();
            await content.CopyToAsync(buffered, cancellationToken);
            content = buffered;
        }

        content.Position = 0;

        var header = new byte[ImageSignature.HeaderLength];
        var read = await content.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, cancellationToken);
        content.Position = 0;

        return (ImageSignature.DetectExtension(header.AsSpan(0, read)), content);
    }
}
