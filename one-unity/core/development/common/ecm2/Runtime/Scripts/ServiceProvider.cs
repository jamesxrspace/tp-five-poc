using EasyCharacterMovement;
using UnityEngine;

namespace TPFive.Extended.ECM2
{
    using GameActor = TPFive.Game.Actor;

    public sealed partial class ServiceProvider :
        GameActor.IServiceProvider
    {
        //
        public void MoveTo(GameObject actor, Vector3 position)
        {
            var result = actor.TryGetComponent(out AgentCharacter agentCharacter);
            if (!result) return;

            agentCharacter.MoveToLocation(position);
        }

        public bool TeleportTo(GameObject actor, Vector3 position)
        {
            return false;
        }
    }
}
