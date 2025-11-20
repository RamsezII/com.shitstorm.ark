using _ARK_;
using System;
using System.Collections;
using System.Collections.Generic;
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
    static void AddTraductable(UnityEditor.MenuCommand command)
    {
        TextMeshProUGUI tmpro = (TextMeshProUGUI)command.context;
        Traductable trad = tmpro.gameObject.AddComponent<Traductable>();

        int index = tmpro.GetComponentIndex();
        while (trad.GetComponentIndex() > index)
            UnityEditorInternal.ComponentUtility.MoveComponentUp(trad);
    }
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
}