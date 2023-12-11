using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Actor
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        void MoveTo(GameObject actor, Vector3 position);
        bool TeleportTo(GameObject actor, Vector3 position);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        void MoveTo(GameObject actor, Vector3 position);
        bool TeleportTo(GameObject actor, Vector3 position);
    }
}
