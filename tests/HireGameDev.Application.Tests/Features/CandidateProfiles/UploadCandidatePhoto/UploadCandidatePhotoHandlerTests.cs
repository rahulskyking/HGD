using HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Application.Tests.Storage;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Features.CandidateProfiles.UploadCandidatePhoto;

public class UploadCandidatePhotoHandlerTests : IDisposable
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0, 1, 2, 3];
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0, 9];
    private static readonly byte[] Html = "<html><script>alert(1)</script></html>"u8.ToArray();

    private readonly SqliteDatabase _database = new();
    private readonly FakeFileStorage _storage = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Stores_a_valid_image_and_records_its_url()
    {
        var userId = await SeedProfileAsync();

        var result = await Handle(userId, Png);

        Assert.True(result.Succeeded);
        var url = Assert.Single(_storage.Files).Key;
        Assert.EndsWith(".png", url);
        Assert.Equal(Png, _storage.Files[url]);

        await using var context = _database.CreateContext();
        Assert.Equal(url, (await context.CandidateProfiles.SingleAsync()).PhotoUrl);
    }

    [Fact]
    public async Task Replacing_a_photo_deletes_the_old_file()
    {
        var userId = await SeedProfileAsync();
        await Handle(userId, Png);
        var firstUrl = _storage.Files.Keys.Single();

        var result = await Handle(userId, Jpeg);

        Assert.True(result.Succeeded);
        Assert.Equal(new[] { firstUrl }, _storage.Deleted);
        Assert.EndsWith(".jpg", _storage.Files.Keys.Single());
    }

    [Fact]
    public async Task Rejects_files_that_are_not_images_whatever_they_claim_to_be()
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

        var result = await Handle(userId, Png, declaredLength: UploadCandidatePhotoCommand.MaxBytes + 1);

        Assert.False(result.Succeeded);
        Assert.Empty(_storage.Files);
    }

    [Fact]
    public async Task Requires_a_saved_profile()
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
        var handler = new UploadCandidatePhotoHandler(context, _storage);

        return await handler.HandleAsync(new UploadCandidatePhotoCommand(userId, content, declaredLength ?? bytes.Length));
    }

    private async Task<Guid> SeedProfileAsync()
    {
        var userId = await _database.AddUserAsync();
        var skills = await _database.AddSkillsAsync("Unity", "C#", "Godot");

        var profile = new CandidateProfile(userId);
        profile.UpdateBasics(
            "Ananya Krishnan", null, "Pune", "Gameplay Programmer", SeniorityLevel.Mid, 4m, null, 1_200_000m,
            NoticePeriod.ThirtyDays, true, OpenToWorkStatus.OpenToOffers, null);
        profile.ReplaceSkills(skills.Select(x => x.Id).ToList());

        await using var context = _database.CreateContext();
        context.CandidateProfiles.Add(profile);
        await context.SaveChangesAsync();

        return userId;
    }
}
