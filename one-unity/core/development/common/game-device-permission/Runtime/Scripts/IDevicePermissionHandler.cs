using Cysharp.Threading.Tasks;

namespace TPFive.Game.DevicePermission
{
    internal interface IDevicePermissionHandler
    {
        bool HasPermission(DevicePermission permission, PermissionType type);

        UniTask<bool> RequestPermission(DevicePermission permission, PermissionType type);
    }

    internal interface IEditorDevicePermissionHandler : IDevicePermissionHandler
    {
        bool IsEnabled { get; }
    }
}