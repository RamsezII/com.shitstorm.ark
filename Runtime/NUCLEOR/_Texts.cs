using _UTIL_;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _ARK_
{
    partial class NUCLEOR : IHomeTexts
    {
        [NJField(editable: false)] string last_user_name;
        [NJField]
        static Languages language = Application.systemLanguage switch
        {
            SystemLanguage.French => Languages.French,
            _ => Languages.English,
        };

        //----------------------------------------------------------------------------------------------------------

        void IArkTexts.OnAfterLoadArkText(in JObject jobj, in bool log)
        {
            Traductable.language.Value = language;
        }
    }
}