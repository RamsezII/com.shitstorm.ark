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
                ui3D_scale = 1,
                ui2D_scale = 1;
        }

        public HSettings hsettings;

        static int ScreenHeightFactor3D => Mathf.Max(1, Mathf.RoundToInt(Screen.height / 600f));
        static int ScreenHeightFactor2D => Mathf.Max(1, Mathf.RoundToInt(Screen.height / 500f));

        int
            lastFactor3D = ScreenHeightFactor3D,
            lastFactor2D = ScreenHeightFactor2D;

        //----------------------------------------------------------------------------------------------------------

        void IHomeTexts.OnSaveHTexts(in bool log)
        {
            hsettings.ui3D_scale = ui3D_pixels_scaler.scaleFactor / lastFactor3D;
            hsettings.ui2D_scale = ui2D.scaler.scaleFactor / lastFactor2D;
            hsettings.SaveStaticJSon(log);
        }

        void IHomeTexts.OnLoadHTexts(in bool log)
        {
            lastFactor3D = ScreenHeightFactor3D;
            lastFactor2D = ScreenHeightFactor2D;
            StaticJSon.ReadStaticJSon(out hsettings, true, log);
            ui3D_pixels_scaler.scaleFactor = hsettings.ui3D_scale * lastFactor3D;
            ui2D.scaler.scaleFactor = hsettings.ui2D_scale * lastFactor2D;
        }
    }
}