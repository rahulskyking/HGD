namespace HireGameDev.Application.Storage;

/// <summary>
/// Where user uploads (profile photos, studio logos) are kept. Callers only ever hold
/// the public URL a save returns; the implementation decides whether that is a local folder
/// or object storage (ADR-016).
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Stores the content under a new unique name inside <paramref name="folder"/> and returns
    /// the public URL path it is served from (e.g. <c>/media/photos/3f2a….jpg</c>).
    /// </summary>
    Task<string> SaveAsync(
        Stream content,
        string folder,
        string extension,
        CancellationToken cancellationToken = default);

    /// <summary>Removes a previously stored file; unknown URLs are ignored.</summary>
    Task DeleteAsync(
        string url,
        CancellationToken cancellationToken = default);
}
