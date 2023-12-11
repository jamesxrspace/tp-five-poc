using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Assist.Entry
{
    using Camera = UnityEngine.Camera;
    using Text = UnityEngine.UI.Text;

    public class UITextPanel : MonoBehaviour
    {
        public Camera cameraFor3D = null;

        protected bool is3D = false;
        protected bool showUI = false;

        [System.NonSerialized]
        protected bool uiCreated = false;

        protected Canvas uiRoot;
        protected Text uiText;

        public bool Is3D
        {
            get
            {
                return is3D;
            }

            set
            {
                if (is3D != value)
                {
                    is3D = value;
                    if (uiCreated)
                    {
                        SetRenderMode();
                        SetUIScale();
                    }
                }
            }
        }

        public bool ShowUI
        {
            get
            {
                return showUI;
            }

            set
            {
                if (showUI != value)
                {
                    showUI = value;
                    if (showUI && !uiCreated)
                    {
                        CreateUI();
                    }

                    uiRoot.gameObject.SetActive(showUI);
                }
            }
        }

        protected virtual Vector2 PanelSize => new Vector2(500f, 500f);

        protected void SetRenderMode()
        {
            if (!is3D)
            {
                uiRoot.renderMode = RenderMode.ScreenSpaceOverlay;
                return;
            }

            var camera = cameraFor3D != null ? cameraFor3D : Camera.main;

            if (camera == null)
            {
                Debug.LogWarning($"{nameof(UIConsole)} CreateUI Warning : Cannot find camera, please set cameraFor3D before create.");
                is3D = false;
                uiRoot.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                uiRoot.renderMode = RenderMode.WorldSpace;
                uiRoot.worldCamera = camera;
            }
        }

        protected void CreateUI()
        {
            var go = gameObject;
            if (!go.TryGetComponent<Canvas>(out uiRoot))
            {
                uiRoot = go.AddComponent<Canvas>();
            }

            uiRoot.vertexColorAlwaysGammaSpace = true;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();

            SetRenderMode();

            var panel = new GameObject("Panel");
            panel.AddComponent<CanvasRenderer>();
            var scrollRect = panel.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            var viewport = new GameObject("Viewport");
            var viewportImage = viewport.AddComponent<Image>();
            var viewportMask = viewport.AddComponent<Mask>();
            viewportImage.color = new Color(0f, 0f, 0f, 0.4f);
            viewportImage.raycastTarget = false;
            viewportImage.maskable = true;

            uiText = new GameObject("Text").AddComponent<Text>();
            uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            uiText.fontSize = 14;
            var sizeFitter = uiText.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform rectTran = go.GetComponent<RectTransform>();
            rectTran.sizeDelta = PanelSize;
            rectTran.localPosition = Vector3.zero;
            rectTran.localScale = is3D ? new Vector3(-0.001f, 0.001f, 1f) : Vector3.one;

            rectTran = panel.GetComponent<RectTransform>();
            rectTran.SetParent(go.transform, false);
            rectTran.localScale = Vector3.one;

            rectTran.anchorMax = new Vector2(0.99f, 0.99f);
            rectTran.anchorMin = new Vector2(0.01f, 0.01f);
            rectTran.offsetMin = Vector3.zero;
            rectTran.offsetMax = Vector3.zero;

            rectTran = viewport.GetComponent<RectTransform>();
            rectTran.SetParent(panel.transform, false);
            rectTran.anchorMax = new Vector2(0.99f, 0.99f);
            rectTran.anchorMin = new Vector2(0.01f, 0.01f);
            rectTran.offsetMin = Vector3.zero;
            rectTran.offsetMax = Vector3.zero;
            scrollRect.viewport = rectTran;

            rectTran = uiText.gameObject.GetComponent<RectTransform>();
            rectTran.SetParent(viewport.transform, false);
            rectTran.localScale = Vector3.one;
            rectTran.anchorMax = new Vector2(0.98f, 0.98f);
            rectTran.anchorMin = new Vector2(0.02f, 0.02f);
            rectTran.offsetMin = Vector3.zero;
            rectTran.offsetMax = Vector3.zero;
            scrollRect.content = rectTran;

            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                go.AddComponent<UnityEngine.EventSystems.EventSystem>();
                go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            uiCreated = true;
        }

        private void SetUIScale()
        {
            RectTransform rectTran = uiRoot.gameObject.GetComponent<RectTransform>();
            rectTran.localScale = is3D ? new Vector3(-0.001f, 0.001f, 1f) : Vector3.one;
            rectTran = uiText.gameObject.GetComponent<RectTransform>();
            rectTran.localScale = Vector3.one;
            rectTran.offsetMin = Vector3.zero;
            rectTran.offsetMax = Vector3.zero;
            rectTran.anchorMax = new Vector2(0.98f, 0.98f);
            rectTran.anchorMin = new Vector2(0.02f, 0.02f);
        }
    }
}
