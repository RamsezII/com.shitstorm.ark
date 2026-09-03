using UnityEngine;
using UnityEngine.UI;

namespace _ARK_
{
    public sealed partial class ArkUI : MonoBehaviour
    {
        public static ArkUI instance;

        public Camera cameraUI;
        [SerializeField] Canvas canvas;
        [SerializeField] CanvasScaler canvasScaler;
        public GraphicRaycaster graphic_raycaster;
        public CanvasGroup canvasGroup;

        public RectTransform
            rt_canvas,
            rt_mode_manager,
            rt_OS_overlay,
            rt_player_ui,
            rt_player_prompt,
            rt_telemetry;

        public interface IPlayerPrompt { }
        public interface IGuiGlobal { }
        public interface IGuiTelemetry { }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var type in Util.EGetAllDerivedTypes<IPlayerPrompt>())
                Util.InstantiateOrCreateIfAbsent(type, parent: rt_player_prompt);

            foreach (var type in Util.EGetAllDerivedTypes<IGuiGlobal>())
                Util.InstantiateOrCreateIfAbsent(type, parent: rt_OS_overlay);

            foreach (var type in Util.EGetAllDerivedTypes<IGuiTelemetry>())
                Util.InstantiateOrCreateIfAbsent(type, parent: rt_telemetry);
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            UsageManager.usages[(int)UsageGroups.IMGUI].AddListener1(isNotEmpty =>
            {
                if (canvasGroup == null)
                {
                    Debug.LogWarning($"{nameof(ArkUI)}.{nameof(canvasGroup)} is null");
                    return;
                }

                canvasGroup.interactable = !isNotEmpty;
                canvasGroup.blocksRaycasts = !isNotEmpty;
            });

            IHomeTexts.AddUser(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        Camera GetEventCamera(Camera eventCamera) => eventCamera != null ? eventCamera : canvas.worldCamera;

        public bool ScreenPointToWorldPoint(
            in RectTransform plane,
            in Vector2 screenPoint,
            Camera eventCamera,
            out Vector3 worldPoint
        ) => RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rect: plane,
            screenPoint: screenPoint,
            cam: GetEventCamera(eventCamera),
            out worldPoint
        );

        public bool ScreenPointToLocalPoint(
            in RectTransform space,
            in Vector2 screenPoint,
            Camera eventCamera,
            out Vector2 localPoint
        ) => RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect: space,
            screenPoint: screenPoint,
            cam: GetEventCamera(eventCamera),
            localPoint: out localPoint
        );

        public bool ScreenDeltaToLocal(
            in RectTransform space,
            in Vector2 screenPoint,
            in Vector2 screenDelta,
            Camera eventCamera,
            out Vector2 localDelta
        )
        {
            if (ScreenPointToLocalPoint(space, screenPoint, eventCamera, out Vector2 current)
                && ScreenPointToLocalPoint(space, screenPoint - screenDelta, eventCamera, out Vector2 previous))
            {
                localDelta = current - previous;
                return true;
            }

            localDelta = Vector2.zero;
            return false;
        }

        public bool SetScreenPosition(in RectTransform target, in Vector2 screenPoint, Camera eventCamera = null)
        {
            if (target.parent is not RectTransform plane
                || !ScreenPointToWorldPoint(plane, screenPoint, eventCamera, out Vector3 worldPoint))
                return false;

            target.position = worldPoint;
            return true;
        }

        public Vector3 InverseTransformPoint(in Camera camera, in Vector3 point)
        {
            Vector3 lpos = camera.WorldToViewportPoint(point);
            Rect r = rt_canvas.rect;
            return new Vector3(
                Mathf.LerpUnclamped(r.xMin, r.xMax, lpos.x),
                Mathf.LerpUnclamped(r.yMin, r.yMax, lpos.y),
                lpos.z
            );
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            IHomeTexts.RemoveUser(this);
        }
    }
}
