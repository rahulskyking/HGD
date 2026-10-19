namespace HireGameDev.Application.Features.StudioProfiles.UploadStudioLogo;

/// <summary>
/// A new studio logo. <paramref name="Content"/> is read once; the caller owns the stream.
/// <paramref name="Length"/> is the declared size, checked before any bytes are read.
/// </summary>
public sealed record UploadStudioLogoCommand(
    Guid UserId,
    Stream Content,
    long Length)
{
    public const long MaxBytes = 2 * 1024 * 1024;

    public const string Folder = "logos";
}
