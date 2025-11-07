using System;
using System.Globalization;
using System.IO;

public static class Util_writetimes
{
    public const string folder_time_format = "yyyy-MM-dd_HH-mm-ss";

    //----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Parse "Wed, 05 Nov 2025 22:04:34 GMT" -> UTC
    /// </summary>
    public static bool TryParseNginxMtimeToDate(this string rfc1123, out DateTimeOffset date)
    {
        // "r" = RFC1123 ; fallback explicite "GMT"
        return DateTimeOffset.TryParseExact(
            rfc1123,
            new[] { "r", "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out date
        );
    }

    public static DateTimeOffset ParseNginxMtimeToDate(this string rfc1123) => TryParseNginxMtimeToDate(rfc1123, out DateTimeOffset date) ? date : default;

    /// <summary>
    /// Format UTC -> mtime style (RFC1123 GMT)
    /// ex: "Wed, 05 Nov 2025 22:04:34 GMT"
    /// </summary>
    public static string ToNginxMtimeString(this DateTimeOffset date) => date.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture);

    // Timestamp d’un fichier local (on prend LastWriteTimeUtc)
    public static DateTimeOffset GetFileDate(this FileInfo file)
    {
        DateTime date = file.LastWriteTimeUtc; // DateTime (UTC)
        return new DateTimeOffset(date, TimeSpan.Zero);
    }

    // Timestamp d’un fichier local (on prend LastWriteTimeUtc)
    public static DateTimeOffset GetFolderDate(this DirectoryInfo dir)
    {
        DateTime date = dir.LastWriteTimeUtc; // DateTime (UTC)
        return new DateTimeOffset(date, TimeSpan.Zero);
    }

    /// <summary>
    /// Formate une date UTC en nom de dossier du type 20251105_220434.
    /// </summary>
    public static string DateToFolderName(this DateTimeOffset date) => date.ToUniversalTime().ToString(folder_time_format, CultureInfo.InvariantCulture);
    public static string DateNowToFolderName() => DateToFolderName(DateTimeOffset.UtcNow);

    public static bool TryParseIntoDate(this string input, out string date_segment, out DateTimeOffset date)
    {
        if (input.Length > folder_time_format.Length)
            date_segment = input[^folder_time_format.Length..];
        else
            date_segment = input;

        if (DateTimeOffset.TryParseExact(
            date_segment,
            folder_time_format,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out date
        ))
            return true;

        date_segment = null;
        date = default;
        return false;
    }
}