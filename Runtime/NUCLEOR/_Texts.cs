using UnityEngine;

namespace _ARK_
{
    partial class NUCLEOR
    {
        [HField(editable: false)] string last_user_name;
        [UField]
        static Languages language = Application.systemLanguage switch
        {
            SystemLanguage.French => Languages.French,
            _ => Languages.English,
        };

        //----------------------------------------------------------------------------------------------------------

        protected override void OnAfterLoadFields(bool log)
        {
            base.OnAfterLoadFields(log);

            Traductable.language.Value = language;
        }
    }
}