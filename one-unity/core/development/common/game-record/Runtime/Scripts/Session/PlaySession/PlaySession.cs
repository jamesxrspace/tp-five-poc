namespace TPFive.Game.Record
{
    using System;
    using Microsoft.Extensions.Logging;
    using Stateless;

    public abstract class PlaySession : IDisposable
    {
        private readonly ILogger logger;

        public PlaySession(ILogger logger)
        {
            this.logger = logger;
            InitStateMachine();
        }

        protected enum Event
        {
            /// <summary>
            /// The state where the game play is being Play.
            /// </summary>
            Play,

            /// <summary>
            /// The state where the play session has been stopped.
            /// </summary>
            Stop,

            /// <summary>
            /// The state where the play session has been pause.
            /// </summary>
            Pause,
        }

        protected enum State
        {
            /// <summary>
            /// Initial state.
            /// </summary>
            StandBy,

            /// <summary>
            /// The Paused state of the play state machine.
            /// This state is entered when the play session is not actively playing back a session.
            /// </summary>
            Paused,

            /// <summary>
            /// The playing state of the play state machine.
            /// </summary>
            Playing,
        }

        public ILogger Logger => logger;

        protected StateMachine<State, Event> Machine { get; private set; }

        public abstract void Dispose();

        public abstract void Setup(RecordData[] data);

        public virtual void Start()
        {
            Machine.Fire(Event.Play);
        }

        public virtual void Stop()
        {
            Machine.Fire(Event.Stop);
        }

        public virtual void Pause()
        {
            Machine.Fire(Event.Pause);
        }

        public virtual void Resume()
        {
            Machine.Fire(Event.Play);
        }

        public virtual bool IsStandBy()
        {
            return Machine.State == State.StandBy;
        }

        public abstract float GetDuration();

        private void InitStateMachine()
        {
            Machine = new StateMachine<State, Event>(State.StandBy);
            Machine.Configure(State.Paused)
                .Permit(Event.Play, State.Playing)
                .PermitReentry(Event.Pause);
            Machine.Configure(State.Playing)
                .Permit(Event.Stop, State.StandBy)
                .Permit(Event.Pause, State.Paused);
            Machine.Configure(State.StandBy)
                .Permit(Event.Play, State.Playing)
                .PermitReentry(Event.Pause);
        }
    }
}
