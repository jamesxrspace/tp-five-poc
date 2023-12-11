using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TPFive.Game
{
    public sealed class GameApp
    {
        public static event System.Action OnApplicationQuit;

        public static event System.Action<bool> OnApplicationFocus;

        public static event System.Action<bool> OnApplicationPause;

        public static RuntimePlatform RuntimePlatform
        {
            get
            {
#if OCULUS_VR
                return RuntimePlatform.OculusVR;
#elif VISION_OS
                return RuntimePlatform.VisionOS;
#endif
                return (RuntimePlatform)Application.platform;
            }
        }

        public static Platform Platform
        {
            get
            {
#if UNITY_IOS
                return Platform.iOS;
#elif UNITY_ANDROID && OCULUS_VR
                return Platform.OculusVR;
#elif UNITY_ANDROID
                return Platform.Android;
#elif UNITY_STANDALONE_WIN
                return Platform.Windows;
#elif UNITY_STANDALONE_OSX
                return Platform.MacOS;
#elif VISION_OS
                return Platform.VisionOS;
#else
                return Platform.Undefined;
#endif
            }
        }

        public static PlatformGroup PlatformGroup
        {
            get
            {
                PlatformGroup group = PlatformGroup.Undefined;
                switch (Platform)
                {
                    case Platform.iOS:
                    case Platform.Android:
                        group = PlatformGroup.MobilePhone;
                        break;
                    case Platform.Windows:
                    case Platform.MacOS:
                        group = PlatformGroup.Standalone;
                        break;
                    case Platform.OculusVR:
                    case Platform.VisionOS:
                        group = PlatformGroup.VRHeadset;
                        break;
                    case Platform.Undefined:
                        break;
                }

                return group;
            }
        }

        public static bool IsMobilePhone => PlatformGroup == PlatformGroup.MobilePhone;

        public static bool IsStandalone => PlatformGroup == PlatformGroup.Standalone;

        public static bool IsVRHeadset => PlatformGroup == PlatformGroup.VRHeadset;

        public static bool IsFlutter
        {
            get
            {
#if FLUTTER
                Assert.IsTrue(IsMobilePhone, "PlatformGroup is not Mobile.");
                return true;
#else
                return false;
#endif
            }
        }

        public static PlatformFlags PlatformFlags => PlatformToFlags(Platform);

        public static PlatformFlags PlatformToFlags(Platform platform)
        {
            return (PlatformFlags)(1 << (int)platform);
        }

        public static PlatformFlags PlatformToFlags(params Platform[] platforms)
        {
            return platforms.Aggregate(
                PlatformFlags.None,
                (flag, platform) => flag |= (PlatformFlags)(1 << (int)platform));
        }

        internal static void RaiseOnApplicationQuitEvent()
        {
            OnApplicationQuit?.Invoke();
        }

        internal static void RaiseOnApplicationFocusEvent(bool focus)
        {
            OnApplicationFocus?.Invoke(focus);
        }

        internal static void RaiseOnApplicationPauseEvent(bool pause)
        {
            OnApplicationPause?.Invoke(pause);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearLegacyReferences()
        {
            // Clear EventHandlers
            OnApplicationQuit = null;
            OnApplicationFocus = null;
            OnApplicationPause = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SetupAfterSceneLoad()
        {
            // Create EventTrigger
            var go = new GameObject(
                nameof(UnityEventTrigger),
                typeof(UnityEventTrigger))
            {
                hideFlags = HideFlags.DontSave,
            };
            Object.DontDestroyOnLoad(go);
        }
    }
}
