#if UNITY_ANDROID
using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine.Android;
using GameLoggingUtility = TPFive.Game.Logging.Utility;

namespace TPFive.Game.DevicePermission
{
    internal class AndroidDevicePermissionHandler : IDevicePermissionHandler
    {
        public AndroidDevicePermissionHandler(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<AndroidDevicePermissionHandler>(loggerFactory);
        }

        private ILogger Logger { get; }

        public bool HasPermission(DevicePermission devicePermission, PermissionType permissionType)
        {
            switch (devicePermission)
            {
                case DevicePermission.Microphone:
                case DevicePermission.Camera:
                    {
                        if (permissionType == PermissionType.Write)
                        {
                            throw new ArgumentException($"Invalid {nameof(PermissionType)}.{permissionType} for {devicePermission}.");
                        }

                        var permission = ToAndroidPermission(devicePermission);
                        return Permission.HasUserAuthorizedPermission(permission);
                    }

                case DevicePermission.PhotoGallery:
                    {
                        var type = permissionType == PermissionType.Read ?
                            NativeGallery.PermissionType.Read :
                            NativeGallery.PermissionType.Write;
                        var permission = NativeGallery.CheckPermission(
                            type,
                            NativeGallery.MediaType.Image);
                        return permission == NativeGallery.Permission.Granted;
                    }

                default:
                    return false;
            }
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

            if (devicePermission == DevicePermission.PhotoGallery)
            {
                var result = NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
                return await UniTask.FromResult(result == NativeGallery.Permission.Granted);
            }

            var permission = ToAndroidPermission(devicePermission);
            var utcs = new UniTaskCompletionSource<AndroidPermisionRequestState>();
            void OnPermissionGranted(string name)
            {
                if (name.Equals(permission, StringComparison.Ordinal))
                {
                    utcs.TrySetResult(AndroidPermisionRequestState.Granted);
                }
            }

            void OnPermissionDenied(string name)
            {
                if (name.Equals(permission, StringComparison.Ordinal))
                {
                    utcs.TrySetResult(AndroidPermisionRequestState.Denied);
                }
            }

            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += OnPermissionGranted;
            callbacks.PermissionDenied += OnPermissionDenied;
            Permission.RequestUserPermission(permission, callbacks);
            var state = await utcs.Task;

            return state == AndroidPermisionRequestState.Granted;
        }

        private static string ToAndroidPermission(DevicePermission devicePermission)
        {
            return devicePermission switch
            {
                DevicePermission.Microphone => Permission.Microphone,
                DevicePermission.Camera => Permission.Camera,
                _ => throw new ArgumentException($"Can not convert {devicePermission} to {typeof(Permission).FullName}"),
            };
        }
    }
}
#endif