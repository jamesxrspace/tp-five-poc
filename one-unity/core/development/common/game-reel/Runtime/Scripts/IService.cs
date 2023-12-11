using System.Threading;
using Cysharp.Threading.Tasks;
using ReelModel = TPFive.OpenApi.GameServer.Model.Reel;

namespace TPFive.Game.Reel
{
    public interface IService
    {
        public UniTask<ReelModel> CreateReel(CreateReelData reelData, CancellationToken cancellationToken = default);

        public UniTask<bool> PublishReel(string reelId, CancellationToken cancellationToken = default);

        public UniTask<bool> DeleteReel(string reelId, CancellationToken cancellationToken = default);
    }
}