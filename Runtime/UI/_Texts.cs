using System;

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

        //----------------------------------------------------------------------------------------------------------

        void IHomeTexts.OnSaveHTexts(in bool log)
        {
            hsettings.ui3D_pixels_scale = ui3D_pixels.scaleFactor;
            hsettings.ui2D_scale = ui2D.canvas.scaleFactor;
            hsettings.SaveStaticJSon(log);
        }

        void IHomeTexts.OnLoadHTexts(in bool log)
        {
            StaticJSon.ReadStaticJSon(out hsettings, true, log);
            ui3D_pixels.scaleFactor = hsettings.ui3D_pixels_scale;
            ui2D.canvas.scaleFactor = hsettings.ui2D_scale;
        }
    }
}