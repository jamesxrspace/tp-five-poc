#if !UNITY_ANDROID
using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using GameLoggingUtility = TPFive.Game.Logging.Utility;

namespace TPFive.Game.DevicePermission
{
    internal class DefaultDevicePermissionHandler : IDevicePermissionHandler
    {
        public DefaultDevicePermissionHandler(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<DefaultDevicePermissionHandler>(loggerFactory);

            // Access Microphone to avoid the problem of never being able to get microphone permission on iOS devices
            if (Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)
            {
                _ = Microphone.devices;
            }
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

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

                        var authorization = ToUserAuthorization(devicePermission);
                        return Application.HasUserAuthorization(authorization);
                    }

                case DevicePermission.PhotoGallery:
                    {
                        if (Application.platform != UnityEngine.RuntimePlatform.IPhonePlayer &&
                            Application.platform != UnityEngine.RuntimePlatform.Android)
                        {
                                throw new NotImplementedException();
                        }

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

            switch (devicePermission)
            {
                case DevicePermission.Microphone:
                case DevicePermission.Camera:
                    var authorization = ToUserAuthorization(devicePermission);
                    await Application.RequestUserAuthorization(authorization);
                    break;
                case DevicePermission.PhotoGallery:
                    if (Application.platform != UnityEngine.RuntimePlatform.IPhonePlayer &&
                        Application.platform != UnityEngine.RuntimePlatform.Android)
                    {
                        throw new NotImplementedException();
                    }

                    NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
                    break;
            }

            bool result = HasPermission(devicePermission, permissionType);
            return await UniTask.FromResult(result);
        }

        private static UserAuthorization ToUserAuthorization(DevicePermission devicePermission) =>
            devicePermission switch
            {
                DevicePermission.Microphone => UserAuthorization.Microphone,
                DevicePermission.Camera => UserAuthorization.WebCam,
                _ => throw new ArgumentException($"Can not convert {devicePermission} to {typeof(UserAuthorization).FullName}"),
            };
    }
}
#endif