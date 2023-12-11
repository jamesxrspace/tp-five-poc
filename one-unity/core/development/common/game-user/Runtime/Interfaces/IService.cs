using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TPFive.Game.User
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask<User> GetUserAsync();

        UniTask<IAvatarProfile> GetAvatarProfile(string userId, CancellationToken token);

        UniTask<int> GetAvatarProfiles(List<string> userIds, List<IAvatarProfile> result, CancellationToken token);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        UniTask<User> GetUserProfileAsync();

        UniTask<IAvatarProfile> GetAvatarProfile(string userId, CancellationToken token);

        UniTask<int> GetAvatarProfiles(List<string> userIds, List<IAvatarProfile> result, CancellationToken token);
    }
}
