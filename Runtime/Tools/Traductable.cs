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
            switch (ArkMachine.language._value)
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

        [Min(0)] public float autowidth_min;
        public RectTransform autowidth_target;

        public Vector2 autosize_min;
        public RectTransform autosize_target;

        public Traductions traductions;
        [HideInInspector] public TextMeshProUGUI tmpro;
        public Action onRefresh;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AfterSceneLoad()
        {
            ArkMachine.language.AddListener(langage =>
            {
                foreach (Traductable self in instances)
                    self.Refresh();
            });
        }

        //----------------------------------------------------------------------------------------------------------

        [ContextMenu(nameof(AutoWidthMin))]
        void AutoWidthMin()
        {
            autowidth_min = GetComponent<TMP_Text>().rectTransform.sizeDelta.x;
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            tmpro = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
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
            if (!didStart)
                return;

            if (tmpro == null)
                Debug.LogError($"no {nameof(tmpro)} on {transform.GetPath(true)}", this);

            string text = traductions.GetAutomatic();
            if (string.IsNullOrWhiteSpace(text))
                text = traductions.english;

            tmpro.text = text;

            if (autosize_target != null)
            {
                var pref = tmpro.GetPreferredValues(
                    text: text,
                    width: float.MaxValue,
                    height: float.MaxValue
                );
                autosize_target.sizeDelta = new(Mathf.Max(pref.x, autosize_min.x), Mathf.Max(pref.y, autosize_min.y));
            }
            else if (autowidth_target != null)
            {
                float width = tmpro.GetPreferredValues(
                    text: text,
                    width: float.MaxValue,
                    height: autowidth_target.rect.height
                ).x;
                autowidth_target.sizeDelta = new(Mathf.Max(width, autowidth_min), autowidth_target.sizeDelta.y);
            }

            onRefresh?.Invoke();
        }

        public void SetOnOff(bool on) => SetTraductions(on ? new("On") : new("Off"));
        public void SetYesNo(bool yes) => SetTraductions(yes ? new() { french = "Oui", english = "Yes", } : new() { french = "Non", english = "No", });
        public void SetText(string text) => SetTraductions(new Traductions(text));
        public void SetTraductions(in Traductions traductions)
        {
            this.traductions = traductions;
            Refresh();
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy() => instances.Remove(this);
    }
}