using _UTIL_;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _ARK_
{
    partial class NUCLEOR : IHomeTexts
    {
        [NJText] string last_user_name;
        [NJEdit]
        static Languages language = Application.systemLanguage switch
        {
            SystemLanguage.French => Languages.French,
            _ => Languages.English,
        };

        //----------------------------------------------------------------------------------------------------------

        void IArkTexts.OnSaveArkText(in JObject jobj, in bool log)
        {
            language = Traductable.language._value;
        }

        void IArkTexts.OnLoadArkText(in JObject jobj, in bool log)
        {
            Traductable.language.Value = language;
        }
    }
}