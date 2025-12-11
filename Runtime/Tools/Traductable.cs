using _UTIL_;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _ARK_
{
    public enum Languages : byte
    {
        English,
        French,
        _last_,
    }

    [Serializable]
    public struct Traductions
    {
        public string english, french;
        public readonly bool IsDefault => string.IsNullOrEmpty(english) && string.IsNullOrEmpty(french);

        //--------------------------------------------------------------------------------------------------------------

        public Traductions(in string all)
        {
            english = all;
            french = all;
        }

        //--------------------------------------------------------------------------------------------------------------

        public override readonly string ToString() => GetAutomatic();
        public readonly string GetAutomatic()
        {
            switch (Traductable.language._value)
            {
                case Languages.French:
                    if (string.IsNullOrEmpty(french))
                        goto default;
                    else
                        return french;
                default:
                    return english;
            }
        }
    }

    public sealed class Traductable : MonoBehaviour
    {
        static readonly HashSet<Traductable> instances = new();
        public static readonly ValueHandler<Languages> language = new();

        public TextMeshProUGUI tmpro;
        public bool auto_width;
        public Traductions traductions;

        Vector2 init_size;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            language.Reset();
            language.AddListener(langage =>
            {
                foreach (Traductable self in instances)
                    self.Refresh();
            });
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            tmpro = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            init_size = tmpro.rectTransform.sizeDelta;
            instances.Add(this);
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            Refresh();
        }

        //----------------------------------------------------------------------------------------------------------

        void Refresh()
        {
            if (!didAwake)
                return;

            if (tmpro == null)
                Debug.LogError($"no {nameof(tmpro)} on {transform.GetPath(true)}", this);

            string text = traductions.GetAutomatic();

            if (string.IsNullOrWhiteSpace(text))
                text = traductions.english;

            tmpro.text = text;

            if (auto_width)
            {
                float w = tmpro.GetPreferredValues(
                    text: text,
                    width: init_size.x,
                    height: float.PositiveInfinity
                ).x;
                w = Mathf.Min(w, init_size.x);
                tmpro.rectTransform.sizeDelta = new(w, 0);
            }
        }

        public void SetTrads(in Traductions traductions)
        {
            this.traductions = traductions;
            Refresh();
        }

        public void SetTrad(string text) => SetTrads(new Traductions { english = text, french = text });

        [Obsolete]
        public void SetTrads_old(in string fr, in string en) => SetTrads(new Traductions { english = en, french = fr });

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy() => instances.Remove(this);
    }
}