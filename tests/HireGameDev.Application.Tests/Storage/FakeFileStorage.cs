using HireGameDev.Application.Storage;

namespace HireGameDev.Application.Tests.Storage;

/// <summary>Remembers what was saved and deleted instead of touching a disk.</summary>
internal sealed class FakeFileStorage : IFileStorage
{
    private int _counter;

    public Dictionary<string, byte[]> Files { get; } = [];

    public List<string> Deleted { get; } = [];

    public async Task<string> SaveAsync(
        Stream content,
        string folder,
        string extension,
        CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        var url = $"/media/{folder}/file-{++_counter}{extension}";
        Files[url] = buffer.ToArray();

        return url;
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        Files.Remove(url);
        Deleted.Add(url);

        return Task.CompletedTask;
    }
}
