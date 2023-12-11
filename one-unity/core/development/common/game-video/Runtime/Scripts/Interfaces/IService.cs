using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Video
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask<IVideoPlayer> CreateVideoPlayer(Transform parent = null);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        UniTask<IVideoPlayer> CreateVideoPlayer(Transform parent = null);
    }
}
