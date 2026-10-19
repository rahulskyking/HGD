using HireGameDev.Application.Features.StudioProfiles.UploadStudioLogo;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Application.Tests.Storage;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Features.StudioProfiles.UploadStudioLogo;

public class UploadStudioLogoHandlerTests : IDisposable
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0, 1, 2, 3];
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0, 9];
    private static readonly byte[] Html = "<html><script>alert(1)</script></html>"u8.ToArray();

    private readonly SqliteDatabase _database = new();
    private readonly FakeFileStorage _storage = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Stores_a_valid_logo_and_records_its_url()
    {
        var userId = await SeedProfileAsync();

        var result = await Handle(userId, Png);

        Assert.True(result.Succeeded);
        var url = Assert.Single(_storage.Files).Key;
        Assert.EndsWith("/logos/file-1.png", url);
        Assert.Equal(Png, _storage.Files[url]);

        await using var context = _database.CreateContext();
        Assert.Equal(url, (await context.StudioProfiles.SingleAsync()).LogoUrl);
    }

    [Fact]
    public async Task Replacing_a_logo_deletes_the_old_file_after_the_new_url_is_saved()
    {
        var userId = await SeedProfileAsync();
        await Handle(userId, Png);
        var firstUrl = _storage.Files.Keys.Single();

        var result = await Handle(userId, Jpeg);

        Assert.True(result.Succeeded);
        Assert.Equal(new[] { firstUrl }, _storage.Deleted);
        Assert.EndsWith(".jpg", _storage.Files.Keys.Single());
        await using var context = _database.CreateContext();
        Assert.Equal(_storage.Files.Keys.Single(), (await context.StudioProfiles.SingleAsync()).LogoUrl);
    }

    [Fact]
    public async Task Rejects_files_that_are_not_images_regardless_of_their_claimed_type()
    {
        var userId = await SeedProfileAsync();

        var result = await Handle(userId, Html);

        Assert.False(result.Succeeded);
        Assert.Empty(_storage.Files);
    }

    [Fact]
    public async Task Rejects_files_over_the_size_limit_before_reading_them()
    {
        var userId = await SeedProfileAsync();

        var result = await Handle(userId, Png, declaredLength: UploadStudioLogoCommand.MaxBytes + 1);

        Assert.False(result.Succeeded);
        Assert.Empty(_storage.Files);
    }

    [Fact]
    public async Task Requires_a_saved_studio_profile()
    {
        var userId = await _database.AddUserAsync();

        var result = await Handle(userId, Png);

        Assert.False(result.Succeeded);
        Assert.Empty(_storage.Files);
    }

    private async Task<OperationResult> Handle(Guid userId, byte[] bytes, long? declaredLength = null)
    {
        await using var context = _database.CreateContext();
        using var content = new MemoryStream(bytes);
        var handler = new UploadStudioLogoHandler(context, _storage);

        return await handler.HandleAsync(new UploadStudioLogoCommand(
            userId, content, declaredLength ?? bytes.Length));
    }

    private async Task<Guid> SeedProfileAsync()
    {
        var userId = await _database.AddUserAsync();
        var profile = new StudioProfile(userId);
        profile.UpdateBasics(
            "Nodding Heads Games", null, "Pune", StudioSize.From11To50,
            StudioType.GameStudio, null, null);

        await using var context = _database.CreateContext();
        context.StudioProfiles.Add(profile);
        await context.SaveChangesAsync();

        return userId;
    }
}
