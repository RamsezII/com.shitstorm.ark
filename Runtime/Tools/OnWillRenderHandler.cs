using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public sealed class OnWillRenderHandler : MonoBehaviour
    {
        public static readonly Dictionary<Camera, HashSet<OnWillRenderHandler>> all_rendered = new();

#if HAS_RP
        static Camera current_camera;
#endif

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            all_rendered.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.Update_OnStartOfFrame += static () =>
            {
                foreach (var pair in all_rendered)
                    pair.Value.Clear();
            };

#if HAS_RP
            UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering -= BeginCam;
            UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering += BeginCam;

            UnityEngine.Rendering.RenderPipelineManager.endCameraRendering -= EndCam;
            UnityEngine.Rendering.RenderPipelineManager.endCameraRendering += EndCam;

            static void BeginCam(UnityEngine.Rendering.ScriptableRenderContext ctx, Camera cam)
            {
                current_camera = cam;
            }

            static void EndCam(UnityEngine.Rendering.ScriptableRenderContext ctx, Camera cam)
            {
                if (current_camera == cam)
                    current_camera = null;
            }
#endif
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnWillRenderObject()
        {
            try
            {
                Camera cam = Camera.current;
#if HAS_RP
                if (cam == null)
                    cam = current_camera;
#endif

                if (cam == null)
                    return;

                var key = this;

                if (all_rendered.TryGetValue(cam, out var set))
                    set.Add(key);
                else
                    all_rendered.Add(cam, new() { key, });
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}