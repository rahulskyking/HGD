using HireGameDev.Application.Features.StudioProfiles.RemoveStudioLogo;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Application.Tests.Storage;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Features.StudioProfiles.RemoveStudioLogo;

public class RemoveStudioLogoHandlerTests : IDisposable
{
    private readonly SqliteDatabase _database = new();
    private readonly FakeFileStorage _storage = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Clears_the_logo_url_and_deletes_the_file()
    {
        var userId = await _database.AddUserAsync();
        var url = await SaveFakeLogoAsync();
        var profile = new StudioProfile(userId);
        profile.UpdateBasics("Nodding Heads Games", null, "Pune", StudioSize.From11To50,
            StudioType.GameStudio, null, null);
        profile.SetLogo(url);

        await using (var setup = _database.CreateContext())
        {
            setup.StudioProfiles.Add(profile);
            await setup.SaveChangesAsync();
        }

        await using (var context = _database.CreateContext())
        {
            await new RemoveStudioLogoHandler(context, _storage)
                .HandleAsync(new RemoveStudioLogoCommand(userId));
        }

        Assert.Equal(new[] { url }, _storage.Deleted);
        Assert.Empty(_storage.Files);
        await using var verification = _database.CreateContext();
        Assert.Null((await verification.StudioProfiles.SingleAsync()).LogoUrl);
    }

    [Fact]
    public async Task Does_nothing_when_the_studio_has_no_logo()
    {
        var userId = await _database.AddUserAsync();
        var profile = new StudioProfile(userId);
        profile.UpdateBasics("Nodding Heads Games", null, "Pune", StudioSize.From11To50,
            StudioType.GameStudio, null, null);

        await using (var setup = _database.CreateContext())
        {
            setup.StudioProfiles.Add(profile);
            await setup.SaveChangesAsync();
        }

        await using (var context = _database.CreateContext())
        {
            await new RemoveStudioLogoHandler(context, _storage)
                .HandleAsync(new RemoveStudioLogoCommand(userId));
        }

        Assert.Empty(_storage.Deleted);
    }

    private async Task<string> SaveFakeLogoAsync()
    {
        using var content = new MemoryStream([1, 2, 3]);
        return await _storage.SaveAsync(content, "logos", ".png");
    }
}
