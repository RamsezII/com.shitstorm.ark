using UnityEngine;

namespace _ARK_
{
    partial class ArkUI
    {
        [UField, SerializeField] float UI_scale = 1;

        //----------------------------------------------------------------------------------------------------------

        protected override void OnAfterLoadFields(bool log)
        {
            base.OnAfterLoadFields(log);

            canvasScaler.scaleFactor = Mathf.Max(1, UI_scale);
        }
    }
}