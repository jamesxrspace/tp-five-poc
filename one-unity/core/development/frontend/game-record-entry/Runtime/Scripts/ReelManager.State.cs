using Microsoft.Extensions.Logging;
using Stateless;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager : MonoBehaviour
    {
        private StateMachine<Mode, ModeCommand> machine;

        private enum Mode
        {
            /// <summary>
            /// Initial state.
            /// </summary>
            Initial,

            /// <summary>
            /// Loading scene and avatars.
            /// </summary>
            Loading,

            /// <summary>
            /// Ready to play or record.
            /// </summary>
            StandBy,

            /// <summary>
            /// Recording.
            /// </summary>
            Recording,

            /// <summary>
            /// End of playing or recording.
            /// </summary>
            Done,
        }

        private enum ModeCommand
        {
            /// <summary>
            /// Load scene and avatars.
            /// </summary>
            Load,

            /// <summary>
            /// Everything is ready.
            /// </summary>
            Loaded,

            /// <summary>
            /// Start recording.
            /// </summary>
            Record,

            /// <summary>
            /// Finish playing or recording.
            /// </summary>
            Finish,
        }

        private void InitStateMachine()
        {
            machine = new StateMachine<Mode, ModeCommand>(Mode.Initial);
            machine.Configure(Mode.Initial)
                .Permit(ModeCommand.Load, Mode.Loading);
            machine.Configure(Mode.Loading)
                .Permit(ModeCommand.Loaded, Mode.StandBy)
                .OnEntry(e =>
                {
                    RenewCancel();
                });
            machine.Configure(Mode.StandBy)
                .Permit(ModeCommand.Record, Mode.Recording);
            machine.Configure(Mode.Recording)
                .Permit(ModeCommand.Finish, Mode.Done);
            machine.Configure(Mode.Done)
                .Permit(ModeCommand.Record, Mode.Recording);
            machine.OnTransitioned(OnMachineTransitioned);
        }

        private void OnMachineTransitioned(StateMachine<Mode, ModeCommand>.Transition transition)
        {
            log.LogDebug($"Transitioned from {transition.Source} to {transition.Destination}.");
        }
    }
}
