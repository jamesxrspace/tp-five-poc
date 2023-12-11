using Microsoft.Extensions.Logging;
using VContainer;
using ICameraService = TPFive.Game.Camera.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Camera
{
    public class SimpleCameraRegistry : ICameraRegistry
    {
        private readonly ILogger log;
        private readonly ICameraService cameraService;

        [Inject]
        public SimpleCameraRegistry(ILoggerFactory argLoggerFactory, ICameraService cameraService)
        {
            this.log = argLoggerFactory.CreateLogger<SimpleCameraRegistry>();
            this.cameraService = cameraService;
        }

        public void Register(ICamera camera)
        {
            cameraService.AddCamera(camera);

            log.LogDebug(
                "{Method}: Register {CameraName}",
                nameof(Register),
                (camera != null) ? camera.Name : "NULL");
        }

        public void Unregister(ICamera camera)
        {
            cameraService.RemoveCamera(camera);

            log.LogDebug(
                "{Method}: Unregister {CameraName}",
                nameof(Register),
                (camera != null) ? camera.Name : "NULL");
        }
    }
}
