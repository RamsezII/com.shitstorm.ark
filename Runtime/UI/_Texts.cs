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
                UI_scale = 1;
        }

        public HSettings hsettings;

        //----------------------------------------------------------------------------------------------------------

        void IHomeTexts.OnSaveHTexts(in bool log)
        {
            hsettings.UI_scale = canvasScaler.scaleFactor;
            hsettings.SaveStaticJSon(log);
        }

        void IHomeTexts.OnLoadHTexts(in bool log)
        {
            StaticJSon.ReadStaticJSon(out hsettings, true, log);
            canvasScaler.scaleFactor = Mathf.Max(1, hsettings.UI_scale);
        }
    }
}