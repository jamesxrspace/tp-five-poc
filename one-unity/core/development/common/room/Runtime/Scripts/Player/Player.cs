using System.ComponentModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Microsoft.Extensions.Logging;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Factory;
using UnityEngine;
using VContainer;
using IUserService = TPFive.Game.User.IService;

namespace TPFive.Room
{
    public interface IPlayer
    {
        string XRId { get; }

        bool IsReady { get; }

        Transform Transform { get; }

        bool IsLocalPlayer { get; }

        bool HasStateAuthority { get; }

        bool HasInputAuthority { get; }

        bool IsProxy { get; }

        string SpaceID { get; }

        string RoomID { get; }
    }

    public sealed class Player : NetworkBehaviour, IPlayer, IAfterSpawned
    {
        [SerializeField]
        private GameObject hudViewPrefab;
        [SerializeField]
        private GameObject avatar;
        [SerializeField]
        private PlayerAttributes attributes;
        [SerializeField]
        private AvatarLoader avatarLoader;
        [SerializeField]
        private Transform cachedTransform;

        private IPlayerSystem playerSystem;
        private IUserService userService;
        private IRoomUserRegistrar roomUserRegistrar;
        private Microsoft.Extensions.Logging.ILogger logger;
        private PlayerHudView hudView;
        private string xrId = string.Empty;

        public string XRId => xrId;

        public bool IsReady => Object != null && Object.IsValid && !string.IsNullOrEmpty(XRId);

        public Transform Transform => cachedTransform;

        public bool IsLocalPlayer => IsReady && Object.HasInputAuthority;

        public string SpaceID => GetSpaceID();

        public string RoomID => GetRoomID();

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IPlayerSystem playerSystem,
            IUserService userService,
            IRoomUserRegistrar roomUserRegistrar)
        {
            this.playerSystem = playerSystem;
            this.userService = userService;
            this.roomUserRegistrar = roomUserRegistrar;
            logger = loggerFactory.CreateLogger<Player>();
        }

        public void AfterSpawned()
        {
            xrId = attributes.UserId.ToString();
            name = $"Player#{XRId}";
            playerSystem.Register(this);
            if (Runner.IsServer)
            {
                roomUserRegistrar.Register(this);
            }

            attributes.PropertyChanged += OnAttributesPropertyChanged;
            LoadAvatar(destroyCancellationToken).Forget();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            playerSystem.Unregister(this);
            if (Runner.IsServer)
            {
                roomUserRegistrar.Unregister(this);
            }
        }

        public string GetSpaceID()
        {
            if (Runner == null || Runner.SessionInfo == null || Runner.SessionInfo.Properties == null)
            {
                return null;
            }

            if (!Runner.SessionInfo.Properties.TryGetValue(FusionRoom.PropertyNameSpaceID, out var value) || !value.IsString)
            {
                return null;
            }

            return value;
        }

        public string GetRoomID()
        {
            if (Runner == null || Runner.SessionInfo == null)
            {
                return null;
            }

            return Runner.SessionInfo?.Name;
        }

        private void OnAttributesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PlayerAttributes.Nickname):
                case nameof(PlayerAttributes.IsMicMute):
                case nameof(PlayerAttributes.IsAway):
                    if (logger.IsEnabled(LogLevel.Debug))
                    {
                        logger.LogDebug("Player#{XRId}: {Property} is updated.", XRId, e.PropertyName);
                    }

                    UpdateHudView();
                    break;
                case nameof(PlayerAttributes.UserId):
                    xrId = attributes.UserId.ToString();
                    break;
            }
        }

        private async UniTask LoadAvatar(CancellationToken token)
        {
            var profile = await userService.GetAvatarProfile(attributes.UserId.Value, token);
            avatarLoader.OnLoaded.RemoveListener(OnAvatarLoaded);
            avatarLoader.OnLoaded.AddListener(OnAvatarLoaded);
            var option = new Options
            {
                Features = Options.FeatureFlags.All,
                EnableToCombineMesh = true,
                EnableToGenerateLod = true,
            };
            option.EnableToLoadBindfile(profile.BinfileUrl);
            await avatarLoader.Load(profile.Format, option, token);
        }

        private void OnAvatarLoaded()
        {
            InitHudAsync();
        }

        private void InitHudAsync()
        {
            var go = Instantiate(hudViewPrefab, avatar.transform);
            go.name = $"PlayerHud#{XRId}";
            if (go.TryGetComponent(out hudView))
            {
                UpdateHudView();
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }

        private void UpdateHudView()
        {
            if (hudView == null)
            {
                return;
            }

            hudView.UpdateLable(attributes.Nickname.Value);
        }

        private void OnDestroy()
        {
            if (hudView != null)
            {
                Destroy(hudView.gameObject);
            }
        }
    }
}
