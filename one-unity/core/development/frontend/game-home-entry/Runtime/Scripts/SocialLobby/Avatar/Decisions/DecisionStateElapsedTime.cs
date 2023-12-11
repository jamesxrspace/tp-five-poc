using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [CreateAssetMenu(menuName = "SocialLobby/Agent/Decision/StateElapsedTime")]
    public class DecisionStateElapsedTime : DecisionBase
    {
        [SerializeField]
        private float requiredElapsedTime = 0.1f;

        public override bool Check(StateController fsm)
        {
            return fsm.StateTimeElapsed >= requiredElapsedTime;
        }
    }
}