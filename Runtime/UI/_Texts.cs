using Newtonsoft.Json.Linq;
using System.IO;

namespace _ARK_
{
    partial class ArkUI
    {
        static readonly string hpath = Path.Combine(ArkMachine.DFHome.FullName, typeof(ArkUI).GetJSonFileName());

        //----------------------------------------------------------------------------------------------------------

        void SaveHText(in bool log)
        {
            JObject jobj = new()
            {
                ["player_ui_scaleFactor"] = ui3D_pixels.scaleFactor,
                ["game_ui_scaleFactor"] = ui2D.canvas.scaleFactor,
            };
            jobj.NJSave(hpath, log: log);
        }

        void LoadHText(in bool log)
        {
            if (!hpath.TryNJRead(out JObject jobj))
                SaveHText(log);
            else
            {
                if (jobj.TryGetValue("player_ui_scaleFactor", out var jtoken))
                    ui3D_pixels.scaleFactor = (float)jtoken;

                if (jobj.TryGetValue("game_ui_scaleFactor", out jtoken))
                    ui2D.canvas.scaleFactor = (float)jtoken;
            }
        }
    }
}