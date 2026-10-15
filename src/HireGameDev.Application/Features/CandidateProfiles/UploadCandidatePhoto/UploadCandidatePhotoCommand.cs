namespace HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;

/// <summary>
/// A new profile photo. <paramref name="Content"/> is read once; the caller owns the stream.
/// <paramref name="Length"/> is the declared size, checked before any bytes are read.
/// </summary>
public sealed record UploadCandidatePhotoCommand(
    Guid UserId,
    Stream Content,
    long Length)
{
    public const long MaxBytes = 2 * 1024 * 1024;

    public const string Folder = "photos";
}
