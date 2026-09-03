using _UTIL_;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _ARK_
{
    partial class ArkUI : IHomeTexts
    {
        [NJField, SerializeField] float UI_scale = 1;

        //----------------------------------------------------------------------------------------------------------

        void IArkTexts.OnBeforeSaveArkText(in JObject jobj, in bool log)
        {
            UI_scale = Mathf.Max(1, canvasScaler.scaleFactor);
        }

        void IArkTexts.OnAfterLoadArkText(in JObject jobj, in bool log)
        {
            canvasScaler.scaleFactor = Mathf.Max(1, UI_scale);
        }
    }
}