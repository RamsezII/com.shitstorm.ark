using System;
using System.Globalization;
using System.IO;

public static class Util_writetimes
{
    public const string folder_time_format = "yyyy-MM-dd_HH-mm";

    //----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Parse "Wed, 05 Nov 2025 22:04:34 GMT" -> UTC
    /// </summary>
    public static bool TryParseNginxMtimeToUtc(this string rfc1123, out DateTimeOffset utc)
    {
        // "r" = RFC1123 ; fallback explicite "GMT"
        return DateTimeOffset.TryParseExact(
            rfc1123,
            new[] { "r", "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out utc
        );
    }

    public static DateTimeOffset ParseNginxMtimeToUtc(this string rfc1123) => TryParseNginxMtimeToUtc(rfc1123, out DateTimeOffset utc) ? utc : default;

    /// <summary>
    /// Format UTC -> mtime style (RFC1123 GMT)
    /// ex: "Wed, 05 Nov 2025 22:04:34 GMT"
    /// </summary>
    public static string ToNginxMtimeString(this DateTimeOffset utc) => utc.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture);

    // Timestamp d’un fichier local (on prend LastWriteTimeUtc)
    public static DateTimeOffset GetFileLastWriteUtc(this string fullPath)
    {
        DateTime dt = File.GetLastWriteTimeUtc(fullPath); // DateTime (UTC)
        return new DateTimeOffset(dt, TimeSpan.Zero);
    }

    // Même chose mais déjà string RFC1123
    public static string GetFileLastWriteUtc_AsMtime(this string file_path) => ToNginxMtimeString(GetFileLastWriteUtc(file_path));

    /// <summary>
    /// Convertit la date de dernière modification d’un fichier local en UTC
    /// puis renvoie le nom de dossier qu’il devrait avoir.
    /// </summary>
    public static string LastFileWriteUtcToFolderName(this string file_path) => LastFileWriteUtcToFolderName(new DateTimeOffset(File.GetLastWriteTimeUtc(file_path), TimeSpan.Zero));

    /// <summary>
    /// Formate une date UTC en nom de dossier du type 20251105_220434.
    /// </summary>
    public static string LastFileWriteUtcToFolderName(this DateTimeOffset utc) => utc.ToUniversalTime().ToString(folder_time_format, CultureInfo.InvariantCulture);
    public static string UtcNowToFolderName() => LastFileWriteUtcToFolderName(DateTimeOffset.UtcNow);

    public static bool TryParsePathNameIntoDate(this string input, out DateTimeOffset utc)
    {
        if (input.Length > folder_time_format.Length)
            input = input[^folder_time_format.Length..];

        return DateTimeOffset.TryParseExact(
            input,
            folder_time_format,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out utc
        );
    }
}