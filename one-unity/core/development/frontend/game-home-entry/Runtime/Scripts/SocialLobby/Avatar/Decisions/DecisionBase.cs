using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public abstract class DecisionBase : ScriptableObject
    {
        public abstract bool Check(StateController fsm);
    }
}