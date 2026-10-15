namespace HireGameDev.Infrastructure.Storage;

/// <summary>
/// Bound from the <c>FileStorage</c> configuration section. Uploads live outside wwwroot so a
/// deployment never serves them as app assets by accident; the Web project maps
/// <see cref="RequestPath"/> to the resolved folder.
/// </summary>
public sealed class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    /// <summary>Folder for uploads; relative paths are resolved against the content root.</summary>
    public string RootPath { get; set; } = "App_Data/media";

    /// <summary>URL prefix the folder is served under.</summary>
    public string RequestPath { get; set; } = "/media";

    public string ResolveRootPath(string contentRootPath) =>
        Path.GetFullPath(Path.Combine(contentRootPath, RootPath));
}
