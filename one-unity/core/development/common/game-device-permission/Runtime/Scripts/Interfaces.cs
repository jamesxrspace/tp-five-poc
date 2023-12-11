using Cysharp.Threading.Tasks;

namespace TPFive.Game.DevicePermission
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        bool HasPermission(DevicePermission permission, PermissionType type = PermissionType.Read);

        UniTask<bool> RequestPermission(DevicePermission permission, PermissionType type = PermissionType.Read);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        bool HasPermission(DevicePermission permission, PermissionType type = PermissionType.Read);

        UniTask<bool> RequestPermission(DevicePermission permission, PermissionType type = PermissionType.Read);
    }
}
