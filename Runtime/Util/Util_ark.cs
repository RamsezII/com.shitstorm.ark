using _ARK_;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public static class Util_ark
{
    public static char GetRotator(in float speed = 10) => ((int)(Time.unscaledTime * speed) % 4) switch
    {
        0 => '|',
        1 => '/',
        2 => '-',
        3 => '\\',
        _ => '?',
    };

    public static IEnumerator<float> ESchedulize(this IEnumerator enumerator, Action<object> onDone = null)
    {
        while (enumerator.MoveNext())
            yield return 0;
        onDone?.Invoke(enumerator.Current);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("CONTEXT/" + nameof(TextMeshProUGUI) + "/" + nameof(AddTraductable))]
    static void AddTraductable(UnityEditor.MenuCommand command) => ((TextMeshProUGUI)command.context).gameObject.AddComponent<Traductable>();
#endif

    public static IEnumerator<float> ERoutinize(this IEnumerable<Action> actions) => ERoutinize(actions.ToArray());
    public static IEnumerator<float> ERoutinize(params Action[] actions)
    {
        float inv = 1f / actions.Length;
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i]?.Invoke();
            yield return (1 + i) * inv;
        }
    }

    public static IEnumerator<float> ERoutinize(this IEnumerable<IEnumerator<float>> routines) => ERoutinize(routines.ToArray());
    public static IEnumerator<float> ERoutinize(params IEnumerator<float>[] routines)
    {
        float inv = 1f / routines.Length;
        for (int i = 0; i < routines.Length; i++)
        {
            using var routine = routines[i];
            while (routine.MoveNext())
                yield return (i + routine.Current) * inv;
        }
    }

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
    public static string LastFileWriteUtcToFolderName(this DateTimeOffset utc) => utc.ToUniversalTime().ToString("yyyy-MM-dd_HH-mm", CultureInfo.InvariantCulture);

    public static bool TryParseFolderNameIntoLastFileWriteUtc(this string folder_name, out DateTimeOffset utc)
    {
        return DateTimeOffset.TryParseExact(
            folder_name,
            "yyyy-MM-dd_HH-mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out utc
        );
    }
}