using HireGameDev.Application.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HireGameDev.Infrastructure.Storage;

/// <summary>
/// Keeps uploads on the server's disk (ADR-016). Files get random names, so nothing a user
/// typed ever becomes part of a path, and deletes refuse anything outside the root folder.
/// </summary>
public sealed class LocalFileStorage(
    IOptions<FileStorageSettings> options,
    IHostEnvironment environment) : IFileStorage
{
    private readonly string _rootPath = options.Value.ResolveRootPath(environment.ContentRootPath);
    private readonly string _requestPath = options.Value.RequestPath.TrimEnd('/');

    public async Task<string> SaveAsync(
        Stream content,
        string folder,
        string extension,
        CancellationToken cancellationToken = default)
    {
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var directory = Path.Combine(_rootPath, folder);

        Directory.CreateDirectory(directory);

        await using (var file = new FileStream(
            Path.Combine(directory, fileName), FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(file, cancellationToken);
        }

        return $"{_requestPath}/{folder}/{fileName}";
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var fullPath = ToFullPath(url);

        if (fullPath is not null && File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>Maps a URL this storage produced back to disk; anything else gives <c>null</c>.</summary>
    private string? ToFullPath(string url)
    {
        if (!url.StartsWith(_requestPath + "/", StringComparison.Ordinal))
        {
            return null;
        }

        var relative = url[(_requestPath.Length + 1)..];
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relative));

        return fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            ? fullPath
            : null;
    }
}
