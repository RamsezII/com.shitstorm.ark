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
            rt_current_mode,
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
                canvasGroup.interactable = !isNotEmpty;
                canvasGroup.blocksRaycasts = !isNotEmpty;
            });

            IHomeTexts.AddUser(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        public bool ScreenPointToWorldPoint(in Vector2 screenPoint, out Vector3 worldPoint) => RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rect: rt_canvas,
            screenPoint: screenPoint,
            cam: cameraUI,
            out worldPoint
        );

        public bool ScreenPointToLocalPoint(in Vector2 screenPoint, out Vector2 localPoint) => RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect: rt_canvas,
            screenPoint: screenPoint,
            cam: cameraUI,
            localPoint: out localPoint
        );

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
