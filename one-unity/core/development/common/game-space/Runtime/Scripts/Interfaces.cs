using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TPFive.Game.Space
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask<List<SpaceGroup>> GetSpaceGroups(CancellationToken cancellationToken);

        UniTask<List<Space>> GetSpaces(string spaceGroupId, CancellationToken cancellationToken);
    }

    public interface IServiceProvider :
        TPFive.Game.IServiceProvider
    {
        UniTask<List<SpaceGroup>> GetSpaceGroups(CancellationToken cancellationToken);

        UniTask<List<Space>> GetSpaces(string spaceGroupId, CancellationToken cancellationToken);
    }
}
