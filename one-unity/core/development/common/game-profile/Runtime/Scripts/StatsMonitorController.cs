using System;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPFive.Game.Profile
{
    public class StatsMonitorController : MonoBehaviour,
     IPointerDownHandler, IPointerUpHandler,
     IPointerClickHandler
    {
        private static readonly Color StatsMonitorInactiveColor = Color.gray;
        private static readonly Color StatsMonitorActiveColor = Color.yellow;
        private static readonly TimeSpan GCPressDuration = TimeSpan.FromSeconds(10);
        private readonly Stopwatch pressStopWatch = new Stopwatch();
        private readonly MemoryMonitor[] memMonitors = new MemoryMonitor[2];
        [SerializeField]
        private GameObject statsMonitor;
        [SerializeField]
        private Image ramModuleBackground;
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private MemoryWarningThreshold memWarningThreshold;
        private Tweener warningTweener;
        private int memWarningCount;

        public bool IsMemWarning => memWarningCount > 0;

        public bool IsShowingMemVisualWarning { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressStopWatch.Start();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressStopWatch.Reset();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                ToggleStatsMonitor();
            }
        }

        protected void Start()
        {
            SetTextColor();
            // Monitor Unity memory usage.
            memMonitors[0] = new MemoryMonitor(MemAllocKind.Unity, memWarningThreshold.UnityAllocated);
            memMonitors[0].OnWarningStatusChanged += OnMemWarningStatusChanged;
            // Monitor Mono memory usage.
            memMonitors[1] = new MemoryMonitor(MemAllocKind.Mono, memWarningThreshold.MonoAllocated);
            memMonitors[1].OnWarningStatusChanged += OnMemWarningStatusChanged;
        }

        protected void Update()
        {
            if (pressStopWatch.IsRunning)
            {
                if (pressStopWatch.Elapsed >= GCPressDuration)
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                #if UNITY_IOS || UNITY_ANDROID
                    Handheld.Vibrate();
                #endif
                    pressStopWatch.Reset();
                }
            }
        }

        private void ToggleStatsMonitor()
        {
            statsMonitor.SetActive(!statsMonitor.activeSelf);
            SetTextColor();
            if (statsMonitor.activeSelf)
            {
                EnableMemMonitors();
            }
            else
            {
                DisableMemMonitors();
                HideMemVisualWarning();
                memWarningCount = 0;
                IsShowingMemVisualWarning = false;
            }
        }

        private void OnMemWarningStatusChanged(MemAllocKind memAllocKind, bool isWarning)
        {
            // Update the count of memory allocation warning
            if (isWarning)
            {
                memWarningCount += 1;
            }
            else
            {
                memWarningCount -= 1;
            }

            // Update the status of visual warning if it's necessary
            if (IsMemWarning != IsShowingMemVisualWarning)
            {
                if (IsMemWarning)
                {
                    ShowMemVisualWarning();
                }
                else
                {
                    HideMemVisualWarning();
                }

                IsShowingMemVisualWarning = IsMemWarning;
            }
        }

        private void SetTextColor()
        {
            text.color = statsMonitor.activeSelf ? StatsMonitorActiveColor : StatsMonitorInactiveColor;
        }

        private void EnableMemMonitors()
        {
            foreach (var memMonitor in memMonitors)
            {
                memMonitor.Start();
            }
        }

        private void DisableMemMonitors()
        {
            foreach (var memMonitor in memMonitors)
            {
                memMonitor.Stop();
            }
        }

        private void ShowMemVisualWarning()
        {
            var warningColor = Color.red;
            // Keep the original background image's alpha
            warningColor.a = ramModuleBackground.color.a;
            warningTweener = ramModuleBackground.DOColor(warningColor, 1).SetLoops(-1).SetEase(Ease.Flash);
        }

        private void HideMemVisualWarning()
        {
            var a = ramModuleBackground.color.a;
            var color = Color.black;
            color.a = a;
            ramModuleBackground.color = color;
            // Release Tweener
            warningTweener?.Kill();
            warningTweener = null;
        }

        [Serializable]
        public struct MemoryWarningThreshold
        {
            [Tooltip("Set the warning threshold of allocated memory used by Unity in MB.")]
            public float UnityAllocated;
            [Tooltip("Set the warning threshold of allocated memory used by Mono in MB.")]
            public float MonoAllocated;
        }
    }
}