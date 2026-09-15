using _UTIL_;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    partial class ArkUI : IHomeTexts
    {
        [NJField, SerializeField] float UI_scale = 1;

        //----------------------------------------------------------------------------------------------------------

        void IArkTexts.OnAfterLoadArkTexts(in Dictionary<Type, JObject> jobjs, in JObject jobj, in bool log)
        {
            canvasScaler.scaleFactor = Mathf.Max(1, UI_scale);
        }
    }
}