using System;
using UnityEngine;

namespace _ARK_
{
    partial class ArkUI : IHomeTexts
    {
        [Serializable]
        public class HSettings : HomeJSon
        {
            public float
                ui3D_pixels_scale = 1,
                ui2D_scale = 1;
        }

        public HSettings hsettings;

        static int ScreenHeightFactor => Mathf.Max(1, Mathf.RoundToInt(Screen.height / 720f + .5f));
        int hfactorWhenLoaded = ScreenHeightFactor;

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(LogScreenResolution))]
        static void LogScreenResolution()
        {
            Debug.Log(Screen.currentResolution);
        }

        [UnityEditor.MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(LogScreenHeight))]
        static void LogScreenHeight()
        {
            Debug.Log(Screen.height);
        }

        [UnityEditor.MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(LogScreenHeightFactor))]
        static void LogScreenHeightFactor()
        {
            Debug.Log(ScreenHeightFactor);
        }
#endif

        void IHomeTexts.OnSaveHTexts(in bool log)
        {
            hsettings.ui3D_pixels_scale = ui3D_pixels.scaleFactor / hfactorWhenLoaded;
            hsettings.ui2D_scale = ui2D.canvas.scaleFactor / hfactorWhenLoaded;
            hsettings.SaveStaticJSon(log);
        }

        void IHomeTexts.OnLoadHTexts(in bool log)
        {
            hfactorWhenLoaded = ScreenHeightFactor;
            StaticJSon.ReadStaticJSon(out hsettings, true, log);
            ui3D_pixels.scaleFactor = hsettings.ui3D_pixels_scale * hfactorWhenLoaded;
            ui2D.canvas.scaleFactor = hsettings.ui2D_scale * hfactorWhenLoaded;
        }
    }
}