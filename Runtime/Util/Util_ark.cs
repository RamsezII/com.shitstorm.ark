using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
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

        public static string GetArkFileName(this Type type) => type.FullName + ArkJSon.arkjson;
        public static string GetArkExtension(this Type type) => "." + GetArkFileName(type);
    }
}