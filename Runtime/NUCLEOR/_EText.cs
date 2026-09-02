#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace _ARK_
{
    partial class NUCLEOR
    {
        const string button_prefixe = "Assets/" + nameof(_ARK_) + "/" + nameof(NUCLEOR) + ".";
        static readonly string editor_texts_fpath = Path.Combine(DFEditorTemp.FullName, typeof(NUCLEOR).GetJSonFileName());

        //----------------------------------------------------------------------------------------------------------

        [MenuItem(button_prefixe + nameof(OpenEText))]
        static void OpenEText()
        {
            SaveEText();
            Application.OpenURL(editor_texts_fpath);
        }

        [MenuItem(button_prefixe + nameof(SaveEText))]
        static void SaveEText()
        {
            JObject jobj = new()
            {
                [nameof(timestamp_editorStart)] = timestamp_editorStart.ToString(),
            };
            jobj.NJSave(editor_texts_fpath);
        }

        [MenuItem(button_prefixe + nameof(LoadEText))]
        static void LoadEText()
        {
            if (!File.Exists(editor_texts_fpath))
                SaveEText();
            else if (editor_texts_fpath.TryNJRead(out JObject jobj))
                if (jobj.TryGetValue(nameof(timestamp_editorStart), out var jtoken))
                    DateTimeOffset.TryParse((string)jtoken, out timestamp_editorStart);

            Debug.Log($"editor timestamp: " + timestamp_editorStart);
        }
    }
}
#endif