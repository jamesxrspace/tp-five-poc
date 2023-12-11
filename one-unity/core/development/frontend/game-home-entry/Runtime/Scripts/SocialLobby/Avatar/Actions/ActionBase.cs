using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public abstract class ActionBase : ScriptableObject
    {
        public abstract void Enter(StateController fsm);

        public abstract void Execute(StateController fsm);

        public abstract void Exit(StateController fsm);
    }
}