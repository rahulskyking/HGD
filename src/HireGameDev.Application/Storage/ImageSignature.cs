namespace HireGameDev.Application.Storage;

/// <summary>
/// Recognises the image formats uploads may have by their leading bytes, so the declared
/// content type and file name are never trusted.
/// </summary>
public static class ImageSignature
{
    public const int HeaderLength = 12;

    /// <summary>Returns the file extension for a supported image, or <c>null</c> when the bytes are not one.</summary>
    public static string? DetectExtension(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return ".jpg";
        }

        if (header.Length >= 8
            && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
            && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
        {
            return ".png";
        }

        if (header.Length >= 12
            && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
            && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P')
        {
            return ".webp";
        }

        return null;
    }

    /// <summary>
    /// Reads the leading bytes of an upload to identify its format, then hands back a stream
    /// rewound to the start so storage receives the whole file. Non-seekable input is buffered.
    /// </summary>
    public static async Task<(string? Extension, Stream Content)> DetectAsync(
        Stream content,
        CancellationToken cancellationToken = default)
    {
        if (!content.CanSeek)
        {
            var buffered = new MemoryStream();
            await content.CopyToAsync(buffered, cancellationToken);
            content = buffered;
        }

        content.Position = 0;

        var header = new byte[HeaderLength];
        var read = await content.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, cancellationToken);
        content.Position = 0;

        return (DetectExtension(header.AsSpan(0, read)), content);
    }
}
