using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public sealed class OnWillRenderHandler : MonoBehaviour
    {
        public static readonly Dictionary<Camera, HashSet<OnWillRenderHandler>> all_visible_handlers = new();

#if HAS_RP
        static Camera current_camera;
#endif

        public new Renderer renderer;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            all_visible_handlers.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
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

        private void Awake()
        {
            renderer = GetComponent<Renderer>();
        }

        //----------------------------------------------------------------------------------------------------------

        static void ClearHandlers()
        {
            foreach (var kvp1 in all_visible_handlers)
                kvp1.Value.Clear();
        }

        public static HashSet<OnWillRenderHandler> GetOrAddCameraVisibles(in Camera camera)
        {
            NUCLEOR.delegates.Update_OnStartOfFrame -= ClearHandlers;
            NUCLEOR.delegates.Update_OnStartOfFrame += ClearHandlers;

            if (all_visible_handlers.TryGetValue(camera, out var handlers))
                return handlers;
            all_visible_handlers.Add(camera, handlers = new());
            return handlers;
        }

        public static void RemoveCameraDict(in Camera camera)
        {
            NUCLEOR.delegates.Update_OnStartOfFrame -= ClearHandlers;
            if (all_visible_handlers.Remove(camera, out var handlers))
                handlers.Clear();
        }

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

                if (all_visible_handlers.TryGetValue(cam, out var handlers))
                    handlers.Add(this);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}