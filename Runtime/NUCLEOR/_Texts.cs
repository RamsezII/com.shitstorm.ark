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

        void IHomeTexts.OnSaveHTexts(in JObject jobj, in bool log)
        {
            language = static_language._value;
        }

        void IHomeTexts.OnLoadHTexts(in JObject jobj, in bool log)
        {
            static_language.Value = language;
        }
    }
}