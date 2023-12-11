using UniRx;

namespace TPFive.Game.Record.Scene
{
    public interface IService
    {
        /// <summary>
        /// Gets information about the current reel scene.
        /// </summary>
        /// <value>information about the current reel scene.</value>
        IReactiveProperty<ReelSceneInfo> SceneInfo { get; }
    }
}
