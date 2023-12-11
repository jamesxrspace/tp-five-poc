using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using VContainer;

namespace TPFive.Game.DevicePermission
{
    using TPFive.Game;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.DevicePermission.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private static readonly int NullServiceProviderIndex = (int)ServiceProviderKind.NullServiceProvider;
        private readonly IDevicePermissionHandler _handler;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
#if UNITY_EDITOR
        private readonly IEditorDevicePermissionHandler _editorHandler;
#endif

        [Inject]
        public Service(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);
            _serviceProviderTable.Add(
                NullServiceProviderIndex,
                new NullServiceProvider((s, args) => Logger.LogDebug(s, args)));
#if UNITY_EDITOR
            _editorHandler = new EditorDevicePermissionHandler(loggerFactory);
#endif

#if UNITY_ANDROID
            _handler = new AndroidDevicePermissionHandler(loggerFactory);
#else
            _handler = new DefaultDevicePermissionHandler(loggerFactory);
#endif
        }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[NullServiceProviderIndex] as IServiceProvider;

        private ILogger Logger { get; }

        public bool HasPermission(DevicePermission devicePermission, PermissionType permissionType)
        {
#if UNITY_EDITOR
            if (_editorHandler.IsEnabled)
            {
                return _editorHandler.HasPermission(devicePermission, permissionType);
            }
#endif
            return _handler.HasPermission(devicePermission, permissionType);
        }

        public async UniTask<bool> RequestPermission(DevicePermission devicePermission, PermissionType permissionType)
        {
            bool? result = null;
#if UNITY_EDITOR
            if (_editorHandler.IsEnabled)
            {
                result = await _editorHandler.RequestPermission(devicePermission, permissionType);
            }
#endif
            result ??= await _handler.RequestPermission(devicePermission, permissionType);

            if (result == true)
            {
                Logger.LogDebug(
                    "{Method}: {DevicePermission} permission for {PermissionType} is granted.",
                    nameof(RequestPermission),
                    devicePermission,
                    Utils.ToDescription(permissionType));
            }
            else
            {
                Logger.LogDebug(
                    "{Method}: User did not grant {DevicePermission} permission for {PermissionType}.",
                    nameof(RequestPermission),
                    devicePermission,
                    Utils.ToDescription(permissionType));
            }

            return result == true;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _compositeDisposable.Dispose();
            }

            _disposed = true;
        }
    }
}