using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IReelSceneService = TPFive.Game.Record.Scene.IService;

namespace TPFive.Game.Record.Scene
{
    /// <summary>
    /// Allow designers to set the <see cref="ReelSceneInfo"/> in the inspector.
    /// Will sets the <see cref="ReelSceneInfo"/> to the <see cref="IReelSceneService"/> at runtime.
    /// </summary>
    [Preserve]
    public sealed class ReelSceneSetup : MonoBehaviour
    {
        private ILogger log;

        [SerializeField]
        private ReelSceneInfo info;

        public ReelSceneInfo Info => info;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, IReelSceneService reelSceneService)
        {
            log = loggerFactory.CreateLogger<ReelSceneSetup>();

            if (reelSceneService == null)
            {
                log.LogInformation(
                    "{Method}: Skip setup reel scene info. Reel scene service is null means current isn't reel scenario",
                    nameof(Construct));
                return;
            }

            reelSceneService.SceneInfo.Value = info;
        }
    }
}
