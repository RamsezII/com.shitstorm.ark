using System;
using UnityEngine;
using UnityEngine.UI;

namespace _ARK_
{
    public sealed partial class ArkUI : MonoBehaviour
    {
        [Serializable]
        public readonly struct ArkCanvas
        {
            public readonly Canvas canvas;
            public readonly GraphicRaycaster raycaster;
            public readonly CanvasGroup canvasGroup;

            public readonly RectTransform
                canvas_rt,
                player_ui,
                current_mode,
                mode_manager,
                OS_overlay;

            //----------------------------------------------------------------------------------------------------------

            internal ArkCanvas(in Canvas canvas)
            {
                this.canvas = canvas;
                canvas_rt = (RectTransform)canvas.transform;
                raycaster = canvas.GetComponent<GraphicRaycaster>();
                canvasGroup = canvas.GetComponent<CanvasGroup>();
                player_ui = (RectTransform)canvas.transform.Find(nameof(player_ui));
                current_mode = (RectTransform)canvas.transform.Find(nameof(current_mode));
                mode_manager = (RectTransform)canvas.transform.Find(nameof(mode_manager));
                OS_overlay = (RectTransform)canvas.transform.Find(nameof(OS_overlay));
            }
        }

        public static ArkUI instance;
        public Camera cameraUI3D;
        public ArkCanvas ui2D, ui3D;

        [SerializeField]
        RectTransform
            rt_telemetry;

        public interface IGuiGlobal { }
        public interface IGuiTelemetry { }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            cameraUI3D = transform.Find("CameraUI3D").GetComponent<Camera>();

            ui2D = new(transform.Find("Canvas2D").GetComponent<Canvas>());
            ui3D = new(cameraUI3D.transform.Find("Canvas3D").GetComponent<Canvas>());

            rt_telemetry = (RectTransform)ui2D.canvas_rt.Find("telemetry");

            foreach (var type in Util.EGetAllDerivedTypes<IGuiGlobal>())
                Util.InstantiateOrCreateIfAbsent(type, parent: ui2D.OS_overlay);

            foreach (var type in Util.EGetAllDerivedTypes<IGuiTelemetry>())
                Util.InstantiateOrCreateIfAbsent(type, parent: rt_telemetry);
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            UsageManager.usages[(int)UsageGroups.IMGUI].AddListener1(isNotEmpty =>
            {
                ui2D.canvasGroup.interactable = ui3D.canvasGroup.interactable = !isNotEmpty;
                ui2D.canvasGroup.blocksRaycasts = ui3D.canvasGroup.blocksRaycasts = !isNotEmpty;
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        public static bool ScreenPointToWorldPoint(in Vector2 screenPoint, out Vector3 worldPoint) => RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rect: instance.ui2D.canvas_rt,
            screenPoint: screenPoint,
            cam: null,
            out worldPoint
        );

        public static bool ScreenPointToLocalPoint(in Vector2 screenPoint, out Vector2 localPoint) => RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect: instance.ui2D.canvas_rt,
            screenPoint: screenPoint,
            cam: null,
            localPoint: out localPoint
        );

        public static Vector3 InverseTransformPoint(in Camera camera, in Vector3 point)
        {
            Vector3 lpos = camera.WorldToViewportPoint(point);
            Rect r = instance.ui2D.canvas_rt.rect;
            return new Vector3(
                Mathf.LerpUnclamped(r.xMin, r.xMax, lpos.x),
                Mathf.LerpUnclamped(r.yMin, r.yMax, lpos.y),
                lpos.z
            );
        }
    }
}
