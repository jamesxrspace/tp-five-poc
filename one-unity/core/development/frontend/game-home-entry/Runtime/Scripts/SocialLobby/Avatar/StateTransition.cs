using System;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [Serializable]
    public class StateTransition
    {
        [SerializeField]
        private DecisionBase decision;
        [SerializeField]
        private State trueState;
        [SerializeField]
        private State falseState;

        public DecisionBase Decision
        {
            get => decision;
            set => decision = value;
        }

        public State TrueState
        {
            get => trueState;
            set => trueState = value;
        }

        public State FalseState
        {
            get => falseState;
            set => falseState = value;
        }
    }
}