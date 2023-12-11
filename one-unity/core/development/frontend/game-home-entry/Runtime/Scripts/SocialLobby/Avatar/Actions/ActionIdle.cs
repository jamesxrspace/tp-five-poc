using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [CreateAssetMenu(menuName = "SocialLobby/Agent/Action/Idle")]
    public class ActionIdle : ActionBase
    {
        public override void Enter(StateController fsm)
        {
        }

        public override void Execute(StateController fsm)
        {
            // idle = do nothing
        }

        public override void Exit(StateController fsm)
        {
        }
    }
}