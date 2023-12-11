using System;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TMPro;
using TPFive.Game;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Motion;
using TPFive.Game.Avatar.Sit;
using TPFive.Game.Avatar.Talk;
using TPFive.Game.Avatar.Tracking;
using TPFive.Game.Mocap;
using TPFive.Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Room
{
    public class AvatarDemoUI : WindowBase
    {
        [SerializeField]
        private Button buttonPrefab;

        [SerializeField]
        private Toggle togglePrefab;

        [SerializeField]
        private GameObject gridLayoutPrefab;

        [SerializeField]
        private Transform headerRoot;

        [SerializeField]
        private Transform contentRoot;

        [SerializeField]
        private TextMeshProUGUI faceStateText;

        [SerializeField]
        private TextMeshProUGUI bodyStateText;

        private ILogger logger;
        private IMotionItem[] motions;
        private IAvatarSitManager sitManager;
        private IAvatarTalkManager talkManager;
        private IAvatarTrackingManager trackingManager;
        private MotionNetworkController motionNetworkController;
        private bool isOpen;
        private bool isSitDown;
        private bool isTalking;
        private bool isFullBodyEnabled;
        private bool isNeckEnabled;
        private bool faceTrackingIsOn;
        private bool bodyTrackingIsOn;
        private UIButton faceTrackingButton;
        private UIButton bodyTrackingButton;

        private bool FaceTrackingIsOn
        {
            get => faceTrackingIsOn;
            set
            {
                if (value == faceTrackingIsOn)
                {
                    return;
                }

                faceTrackingIsOn = value;
                UpdateTrackingMode();
            }
        }

        private bool BodyTrackingIsOn
        {
            get => bodyTrackingIsOn;
            set
            {
                if (value == bodyTrackingIsOn)
                {
                    return;
                }

                bodyTrackingIsOn = value;
                UpdateTrackingMode();
            }
        }

        [Inject]
        public void Construct(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<AvatarDemoUI>();
        }

        protected override void OnCreate(IBundle bundle)
        {
            motionNetworkController = bundle.Get<MotionNetworkController>(nameof(MotionNetworkController));
            bundle.Data.TryGetValue(nameof(IAvatarContextProvider), out var provider);
            var avatarContextProvider = provider as IAvatarContextProvider;

            if (!avatarContextProvider.IsAlive())
            {
                logger.LogError($"{nameof(AvatarContextProvider)} is null");

                return;
            }

            motions = avatarContextProvider.MotionManager.Motions;
            sitManager = avatarContextProvider.SitManager;
            talkManager = avatarContextProvider.TalkManager;
            trackingManager = avatarContextProvider.TrackingManager;
            contentRoot.gameObject.SetActive(false);
            CreateUIs();
        }

        private void CreateUIs()
        {
            CreateButton(
                headerRoot,
                "Demo Dashboard",
                uiBtn =>
                {
                    if (isOpen)
                    {
                        contentRoot.gameObject.SetActive(false);
                        uiBtn.Text = "Demo Dashboard";
                        isOpen = false;
                    }
                    else
                    {
                        contentRoot.gameObject.SetActive(true);
                        uiBtn.Text = "Close Dashboard";
                        isOpen = true;
                    }
                });

            CreateMotionUIs();
            CreateSitUIs();
            CreateTalkUIs();
            CreateTrackingUIs();
        }

        private void CreateMotionUIs()
        {
            var motionRoot = CreateGridArea();

            for (int i = 0; i < motions.Length; i++)
            {
                var motionUid = motions[i].Uid;

                CreateButton(
                    motionRoot,
                    $"PlayMotion {i}",
                    _ => motionNetworkController.PlayMotion(motionUid));
            }

            CreateButton(
                contentRoot,
                "Stop Motion",
                _ => motionNetworkController.StopMotion());
        }

        private void CreateSitUIs()
        {
            CreateButton(
                contentRoot,
                "Start Sit",
                uiBtn =>
                {
                    if (isSitDown)
                    {
                        sitManager.StandUp();
                        uiBtn.Text = "Start Sit";
                        isSitDown = false;
                    }
                    else
                    {
                        sitManager.SitDown();
                        uiBtn.Text = "Stop Sit";
                        isSitDown = true;
                    }
                });
        }

        private void CreateTalkUIs()
        {
            CreateButton(
                contentRoot,
                "Start Talking",
                uiBtn =>
                {
                    if (isTalking)
                    {
                        talkManager.StopTalk();
                        uiBtn.Text = "Start Talking";
                        isTalking = false;
                    }
                    else
                    {
                        talkManager.StartTalk();
                        uiBtn.Text = "Stop Talking";
                        isTalking = true;
                    }
                });
        }

        private void CreateTrackingUIs()
        {
            var trackingRoot = CreateGridArea();

            _ = CreateToggle(
                trackingRoot,
                "Full Body",
                uiToggle => isFullBodyEnabled = uiToggle.IsOn);

            bodyTrackingButton = CreateButton(
                contentRoot,
                "Start Body Tracking",
                uiBtn =>
                {
                    BodyTrackingIsOn = !BodyTrackingIsOn;
                    uiBtn.Text = bodyTrackingIsOn ? "Stop Body Tracking" : "Start Body Tracking";
                    bodyStateText.text = bodyTrackingIsOn ? "Enable" : "Disable";
                });

            faceTrackingButton = CreateButton(
                contentRoot,
                "Start Face Tracking",
                uiBtn =>
                {
                    FaceTrackingIsOn = !FaceTrackingIsOn;
                    uiBtn.Text = faceTrackingIsOn ? "Stop Face Tracking" : "Start Face Tracking";
                    faceStateText.text = faceTrackingIsOn ? "Enable" : "Disable";
                });
        }

        private async void UpdateTrackingMode()
        {
            bodyTrackingButton.Enable = false;
            faceTrackingButton.Enable = false;

            trackingManager.OnFaceTrackingStarted -= OnFaceTrackingStarted;
            trackingManager.OnBodyTrackingStarted -= OnBodyTrackingStarted;

            try
            {
                if (!faceTrackingIsOn && !bodyTrackingIsOn)
                {
                    await trackingManager.StopTracking();
                    return;
                }

                var options = CaptureOptions.None;
                if (faceTrackingIsOn)
                {
                    options.EnableFace();
                }

                if (bodyTrackingIsOn)
                {
                    if (isFullBodyEnabled)
                    {
                        options.EnableFullBody();
                    }
                    else
                    {
                        options.EnableUpperBody();
                    }
                }

                if (options.IsEnableFace)
                {
                    faceStateText.text = "Enable";
                    trackingManager.OnFaceTrackingStarted += OnFaceTrackingStarted;
                }

                if (options.IsEnableBody)
                {
                    bodyStateText.text = "Enable";
                    trackingManager.OnBodyTrackingStarted += OnBodyTrackingStarted;
                }

                await trackingManager.StartTracking(options);

                if (options.IsEnableFace)
                {
                    faceStateText.text = "SystemReady";
                }

                if (options.IsEnableBody)
                {
                    bodyStateText.text = "SystemReady";
                }
            }
            finally
            {
                bodyTrackingButton.Enable = true;
                faceTrackingButton.Enable = true;
            }
        }

        private void OnFaceTrackingStarted()
        {
            trackingManager.OnFaceTrackingStarted -= OnFaceTrackingStarted;
            this.faceStateText.text = "Running";
        }

        private void OnBodyTrackingStarted()
        {
            trackingManager.OnBodyTrackingStarted -= OnBodyTrackingStarted;
            this.bodyStateText.text = "Running";
        }

        private UIButton CreateButton(Transform parent, string text, Action<UIButton> action = null)
        {
            var button = Instantiate(buttonPrefab, parent);
            var uiButton = new UIButton(button, text, action);

            return uiButton;
        }

        private UIToggle CreateToggle(Transform parent, string text, Action<UIToggle> action = null)
        {
            var toggle = Instantiate(togglePrefab, parent);
            toggle.isOn = false;
            var uiToggle = new UIToggle(toggle, text, action);

            return uiToggle;
        }

        private Transform CreateGridArea()
        {
            var gridArea = Instantiate(gridLayoutPrefab, contentRoot);
            gridArea.SetActive(true);

            return gridArea.transform;
        }

        private class UIButton
        {
            private Button button;
            private TextMeshProUGUI textUI;

            public UIButton(Button button, string text, Action<UIButton> action = null)
            {
                this.button = button;
                this.textUI = button.GetComponentInChildren<TextMeshProUGUI>();
                button.onClick.AddListener(() => action?.Invoke(this));
                button.gameObject.SetActive(true);
                Text = text;
            }

            public bool Enable
            {
                get => button.enabled;
                set => button.enabled = value;
            }

            public string Text
            {
                get => textUI.text;
                set => textUI.text = value;
            }
        }

        private class UIToggle
        {
            private Toggle toggle;
            private TextMeshProUGUI textUI;

            public UIToggle(Toggle toggle, string text, Action<UIToggle> action = null)
            {
                this.toggle = toggle;
                textUI = toggle.GetComponentInChildren<TextMeshProUGUI>();
                toggle.onValueChanged.AddListener(_ => action?.Invoke(this));
                toggle.gameObject.SetActive(true);
                Text = text;
            }

            public string Text
            {
                get => textUI.text;
                set => textUI.text = value;
            }

            public bool IsOn
            {
                get => toggle.isOn;
                set
                {
                    if (value == IsOn)
                    {
                        return;
                    }

                    toggle.isOn = value;
                }
            }
        }
    }
}