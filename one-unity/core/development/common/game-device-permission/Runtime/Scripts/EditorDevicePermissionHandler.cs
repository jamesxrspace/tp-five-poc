#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GameLoggingUtility = TPFive.Game.Logging.Utility;

namespace TPFive.Game.DevicePermission
{
    internal class EditorDevicePermissionHandler : IEditorDevicePermissionHandler
    {
        public EditorDevicePermissionHandler(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<EditorDevicePermissionHandler>(loggerFactory);
        }

        public bool IsEnabled
        {
            get => IsEditorPermissionEnabled;
            set => IsEditorPermissionEnabled = value;
        }

        private static bool IsEditorPermissionEnabled
        {
            get => UnityEditor.EditorPrefs.GetBool("IsEditorPermissionEnabled", false);
            set => UnityEditor.EditorPrefs.SetBool("IsEditorPermissionEnabled", value);
        }

        private ILogger Logger { get; }

        public bool HasPermission(DevicePermission devicePermission, PermissionType permissionType)
        {
            if ((devicePermission == DevicePermission.Microphone ||
                devicePermission == DevicePermission.Camera) &&
                permissionType == PermissionType.Write)
            {
                throw new ArgumentException($"Invalid {nameof(PermissionType)}.{permissionType} for {devicePermission}.");
            }

            return HasPermissionInEditor(devicePermission, permissionType);
        }

        public async UniTask<bool> RequestPermission(DevicePermission devicePermission, PermissionType permissionType)
        {
            if (HasPermission(devicePermission, permissionType))
            {
                return await UniTask.FromResult(true);
            }

            Logger.LogDebug(
                "{Method}: Hasn't {DevicePermission} permission for {PermissionType} yet. Request this permission.",
                nameof(RequestPermission),
                devicePermission,
                Utils.ToDescription(permissionType));

            PromptPermissionDialog(devicePermission, permissionType);

            bool result = HasPermission(devicePermission, permissionType);
            return await UniTask.FromResult(result);
        }

        [UnityEditor.MenuItem("TPFive/DevicePermission/Enable Editor Permission")]
        private static void EnableEditorPermission()
        {
            IsEditorPermissionEnabled = true;
        }

        [UnityEditor.MenuItem("TPFive/DevicePermission/Disable Editor Permission")]
        private static void DisableEditorPermission()
        {
            IsEditorPermissionEnabled = false;
        }

        [UnityEditor.MenuItem("TPFive/DevicePermission/Remove All Permission")]
        private static void RemoveAllPermission()
        {
            AllowPermissionInEditor(DevicePermission.Microphone, PermissionType.Read, false);
            AllowPermissionInEditor(DevicePermission.Camera, PermissionType.Read, false);
            AllowPermissionInEditor(DevicePermission.PhotoGallery, PermissionType.Read, false);
            AllowPermissionInEditor(DevicePermission.PhotoGallery, PermissionType.Write, false);
        }

        private static void AllowPermissionInEditor(
            DevicePermission devicePermission,
            PermissionType permissionType,
            bool enabled)
        {
            UnityEditor.EditorPrefs.SetBool($"DevicePermission-{devicePermission}-{permissionType}", enabled);
        }

        private static bool HasPermissionInEditor(DevicePermission devicePermission, PermissionType permissionType)
        {
            return UnityEditor.EditorPrefs.GetBool($"DevicePermission-{devicePermission}-{permissionType}", false);
        }

        private static void PromptPermissionDialog(DevicePermission devicePermission, PermissionType permissionType)
        {
            var enabled = UnityEditor.EditorUtility.DisplayDialog(
                "Permisson Test",
                $"Request {devicePermission} permission for {Utils.ToDescription(permissionType)}",
                "yes",
                "no");
            AllowPermissionInEditor(devicePermission, permissionType, enabled);
        }
    }
}
#endif