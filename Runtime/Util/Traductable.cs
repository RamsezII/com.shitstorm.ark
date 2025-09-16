using _UTIL_;
using System;
using System.Collections.Generic;
using System.IO;
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

        public override readonly string ToString() => Automatic;
        public readonly string Automatic => Traductable.language.Value switch
        {
            Languages.French => string.IsNullOrWhiteSpace(french) ? "[VIDE]" : french,
            _ => string.IsNullOrWhiteSpace(english) ? "[EMPTY]" : english,
        };
    }

    public static partial class Util_nucleor
    {
        public static Traductions ReadTraductions(this BinaryReader reader) => new() { french = reader.ReadString(), english = reader.ReadString(), };

        public static void WriteTraductions(this BinaryWriter writer, in Traductions traductions)
        {
            writer.Write(traductions.french);
            writer.Write(traductions.english);
        }
    }

    public class Traductable : MonoBehaviour
    {
        static readonly HashSet<Traductable> instances = new();
        public static readonly OnValue<Languages> language = new();

        public Traductions traductions;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            language.Reset();
            language.AddListener(langage =>
            {
                foreach (Traductable self in instances)
                    self.Refresh();
            });
        }

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(InvertTrads))]
        void InvertTrads() => (traductions.french, traductions.english) = (traductions.english, traductions.french);

        [ContextMenu(nameof(ApplyTrads))]
        void ApplyTrads() => SetTrads(traductions);
#endif

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instances.Add(this);
            Refresh();
        }

        //----------------------------------------------------------------------------------------------------------

        public IEnumerable<TextMeshProUGUI> AllTmps()
        {
            if (TryGetComponent(out TextMeshProUGUI tmp))
                yield return tmp;
            else
                foreach (TextMeshProUGUI child in GetComponentsInChildren<TextMeshProUGUI>())
                    yield return child;
        }

        //----------------------------------------------------------------------------------------------------------

        void Refresh()
        {
            string text = traductions.ToString();

            if (string.IsNullOrWhiteSpace(text))
                text = traductions.english;

            foreach (TextMeshProUGUI tmp in AllTmps())
                tmp.text = text;
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