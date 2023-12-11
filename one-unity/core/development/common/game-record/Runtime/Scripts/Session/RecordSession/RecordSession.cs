using System.Collections.Generic;

namespace TPFive.Game.Record
{
    using System;
    using Microsoft.Extensions.Logging;
    using Stateless;

    public abstract partial class RecordSession : IDisposable
    {
        private readonly ILogger logger;

        public RecordSession(ILogger logger)
        {
            this.logger = logger;
            InitStateMachine();
        }

        protected enum Event
        {
            /// <summary>
            /// The state where the game record is being recorded.
            /// </summary>
            Record,

            /// <summary>
            /// The state where the record session has been stopped.
            /// </summary>
            Stop,
        }

        protected enum State
        {
            /// <summary>
            /// The idle state of the record state machine.
            /// This state is entered when the record session is not actively recording back a session.
            /// </summary>
            Idle,

            /// <summary>
            /// The recording state of the record state machine.
            /// </summary>
            Recording,
        }

        public ILogger Logger => logger;

        protected StateMachine<State, Event> Machine { get; private set; }

        public abstract void Setup(RecordData[] data);

        public virtual void Start()
        {
            Machine.Fire(Event.Record);
        }

        public virtual void Stop()
        {
            Machine.Fire(Event.Stop);
        }

        public abstract IEnumerable<RecordData> GetRecordData();

        public abstract void Dispose();

        private void InitStateMachine()
        {
            Machine = new StateMachine<State, Event>(State.Idle);
            Machine.Configure(State.Idle)
                .Permit(Event.Record, State.Recording);
            Machine.Configure(State.Recording)
                .Permit(Event.Stop, State.Idle);
        }
    }
}