using Fusion;
using TPFive.Extended.Fusion;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Room
{
    public class PlayerAttributes : ObservableNetworkBehaviour
    {
        private ILogger logger;

        [Networked(OnChanged = nameof(OnUserIdChanged))]
        public NetworkString<_64> UserId { get; set; }

        [Networked(OnChanged = nameof(OnNicknameChanged))]
        public NetworkString<_64> Nickname { get; set; }

        [Networked(OnChanged = nameof(OnIsTalkingChanged))]
        public NetworkBool IsTalking { get; set; }

        [Networked(OnChanged = nameof(OnIsMicMuteChanged))]
        public NetworkBool IsMicMute { get; set; }

        [Networked(OnChanged = nameof(OnIsAwayChanged))]
        public NetworkBool IsAway { get; set; }

        public bool IsLocalPlayer => Object.HasInputAuthority;

        private ILogger Logger => logger ??= CreateLogger<PlayerAttributes>();

        public static void OnUserIdChanged(Changed<PlayerAttributes> changed)
        {
            changed.Behaviour.RaisePropertyChanged(nameof(UserId));
        }

        public static void OnNicknameChanged(Changed<PlayerAttributes> changed)
        {
            changed.Behaviour.RaisePropertyChanged(nameof(Nickname));
        }

        public static void OnIsTalkingChanged(Changed<PlayerAttributes> changed)
        {
            changed.Behaviour.RaisePropertyChanged(nameof(IsTalking));
        }

        public static void OnIsMicMuteChanged(Changed<PlayerAttributes> changed)
        {
            changed.Behaviour.RaisePropertyChanged(nameof(IsMicMute));
        }

        public static void OnIsAwayChanged(Changed<PlayerAttributes> changed)
        {
            changed.Behaviour.RaisePropertyChanged(nameof(IsAway));
        }
    }
}
