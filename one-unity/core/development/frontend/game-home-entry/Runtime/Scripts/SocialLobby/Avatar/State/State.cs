using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [CreateAssetMenu(menuName = "SocialLobby/Agent/State")]
    public class State : ScriptableObject
    {
        [SerializeField]
        private List<ActionBase> actions;
        [SerializeField]
        private StateTransition stateTransition;

        public void Enter(StateController fsm)
        {
            actions.FindAll(x => x != null)
                .ForEach(x => x.Enter(fsm));
        }

        public void Execute(StateController fsm)
        {
            if (stateTransition != null)
            {
                var nextState = stateTransition.Decision.Check(fsm)
                    ? stateTransition.TrueState
                    : stateTransition.FalseState;
                if (nextState != null)
                {
                    fsm.ChangeState(nextState);
                    return;
                }
            }

            actions.FindAll(x => x != null)
                .ForEach(x => x.Execute(fsm));
        }

        public void Exit(StateController fsm)
        {
            actions.FindAll(x => x != null)
                .ForEach(x => x.Exit(fsm));
        }
    }
}