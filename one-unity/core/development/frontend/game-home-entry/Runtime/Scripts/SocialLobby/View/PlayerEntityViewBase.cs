using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TMPro;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.User;
using UnityEngine;
using UnityEngine.UI;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Random = UnityEngine.Random;

namespace TPFive.Home.Entry.SocialLobby
{
    public abstract class PlayerEntityViewBase : View
    {
        [SerializeField]
        private GameObject thumbnailRoot;
        [SerializeField]
        private RawImage thumbnail;
        [SerializeField]
        private TextMeshProUGUI nickname;
        [SerializeField]
        private GameObject loadingIcon;
        [SerializeField]
        private GameObject visualRoot;
        [SerializeField]
        private GameObject bubbleRoot;
        [SerializeField]
        private Vector3 randomAvatarAngleMin;
        [SerializeField]
        private Vector3 randomAvatarAngleMax;
        [SerializeField]
        private Transform pivot;
        [SerializeField]
        private AgentAvatar agentAvatar;
        [SerializeField]
        private GameObject avatarShadow;
        [SerializeField]
        private GameObject myavatarVFX;
        private bool isBinded;
        private GameObject cachedAvatarGameObject;
        private PlayerEntityViewModel viewModel;
        private bool isCreatingAvatar;
        private ScheduledExecutor scheduledExecutor;
        private ILoggerFactory loggerFactory;
        private ILogger logger;

        private ILogger Logger => logger ??= loggerFactory.CreateLogger<PlayerEntityViewMobile>();

        public virtual void Initialize(PlayerEntityViewModel dataContext)
        {
            this.SetDataContext(dataContext);
            viewModel = dataContext;
            scheduledExecutor = viewModel.ScheduledExecutor;
            loggerFactory = viewModel.LoggerFactory;

            if (!isBinded)
            {
                isBinded = true;

                var bindingSet = this.CreateBindingSet<PlayerEntityViewBase, PlayerEntityViewModel>();
                bindingSet.Bind().For(v => v.OnVisibilityChangedRequest).To(vm => vm.OnVisibilityChangedRequest);
                bindingSet.Bind(this.transform).For(v => v.localPosition).To(vm => vm.LocalPosition);
                bindingSet.Bind(thumbnailRoot).For(v => v.activeSelf).ToExpression(vm => vm.Thumbnail != null);
                bindingSet.Bind(bubbleRoot).For(v => v.activeSelf).ToExpression(vm => vm.Thumbnail != null);
                bindingSet.Bind(thumbnail).For(v => v.texture).To(vm => vm.Thumbnail);
                bindingSet.Bind(nickname).For(v => v.text).To(vm => vm.Nickname);
                bindingSet.Bind(nickname.gameObject).For(v => v.activeSelf).ToExpression(vm => !vm.IsMe);

                bindingSet.Build();
            }
        }

        protected virtual void OnDestroy()
        {
            viewModel?.Dispose();
        }

        private void OnVisibilityChangedRequest(object sender, InteractionEventArgs args)
        {
            var isVisible = (bool)args.Context;

            if (isVisible)
            {
                visualRoot.SetActive(true);
                if (avatarShadow)
                {
                    avatarShadow.SetActive(true);
                }

                if (myavatarVFX)
                {
                    myavatarVFX.SetActive(viewModel.IsMe);
                }

                scheduledExecutor.AddTask(TryCreateAvatarGameObject);

                viewModel?.LoadThumbnail().Forget();
            }
            else
            {
                visualRoot.SetActive(false);
                if (avatarShadow)
                {
                    avatarShadow.SetActive(false);
                }

                if (myavatarVFX)
                {
                    myavatarVFX.SetActive(!viewModel.IsMe);
                }

                if (cachedAvatarGameObject != null)
                {
                    Destroy(cachedAvatarGameObject);
                    cachedAvatarGameObject = null;
                }

                StopAgent();

                viewModel?.ReleaseThumbnail();
            }
        }

        private async UniTask TryCreateAvatarGameObject(CancellationToken cancellationToken)
        {
            // prevent create avatar multiple times.
            if (isCreatingAvatar || cachedAvatarGameObject != null)
            {
                return;
            }

            try
            {
                isCreatingAvatar = true;
                loadingIcon.SetActive(true);

                await TryCreateAvatarGameObjectInner(cancellationToken);
            }
            catch (Exception e)
            {
                string profileOwnerId = null;
                if (this.GetDataContext() is PlayerEntityViewModel vm)
                {
                    var avatarProfile = vm.AvatarProfile;
                    profileOwnerId = avatarProfile.OwnerId;
                }

                Logger.LogWarning(
                    $"{nameof(LoadAvatarGameObject)} catch exception, profileOwnerId={profileOwnerId}, exception={e}");
            }
            finally
            {
                isCreatingAvatar = false;
                loadingIcon.SetActive(false);
            }
        }

        private async UniTask TryCreateAvatarGameObjectInner(CancellationToken cancellationToken)
        {
            if (cachedAvatarGameObject != null)
            {
                return;
            }

            if (this.GetDataContext() is PlayerEntityViewModel vm)
            {
                var avatarFactory = vm.AvatarFactory;
                var avatarProfile = vm.AvatarProfile;
                if (avatarProfile == null)
                {
                    Logger.LogWarning($"{nameof(TryCreateAvatarGameObject)} skipped, avatarProfile is null");

                    return;
                }

                var linkedCancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(
                        destroyCancellationToken,
                        cancellationToken);
                await LoadAvatarGameObject(avatarFactory, avatarProfile, linkedCancellationTokenSource.Token)
                    .ContinueWith(
                        isLoadAvatar =>
                        {
                            if (isLoadAvatar)
                            {
                                StartAgent();
                            }
                        });
            }
        }

        private async UniTask<bool> LoadAvatarGameObject(
            IAvatarFactory avatarFactory,
            IAvatarProfile avatarProfile,
            CancellationToken cancellationToken)
        {
            try
            {
                cachedAvatarGameObject = new GameObject("Avatar");

                var transform = cachedAvatarGameObject.transform;
                transform.SetParent(pivot);
                var rotation = Quaternion.Euler(
                    Random.Range(randomAvatarAngleMin.x, randomAvatarAngleMax.x),
                    Random.Range(randomAvatarAngleMin.y, randomAvatarAngleMax.y),
                    Random.Range(randomAvatarAngleMin.z, randomAvatarAngleMax.z));
                transform.SetLocalPositionAndRotation(Vector3.zero, rotation);
                transform.localScale = Vector3.one;

                var avatarFormat = avatarProfile.Format;
                var options = new Options()
                {
                    Features = Options.FeatureFlags.All,
                };
                options.EnableToLoadBindfile(avatarProfile.BinfileUrl);

                var linkedCancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(
                        cachedAvatarGameObject.GetCancellationTokenOnDestroy(),
                        cancellationToken);
                var (isCancelled, isSuccess) = await avatarFactory
                    .Setup(cachedAvatarGameObject, avatarFormat, options, linkedCancellationTokenSource.Token)
                    .SuppressCancellationThrow();

                if (!isSuccess || isCancelled)
                {
                    // handle create avatar failed.
                    if (cachedAvatarGameObject != null)
                    {
                        Destroy(cachedAvatarGameObject);
                    }
                }
            }
            catch
            {
                // handle exception during creating avatar
                // destroy uncompleted avatar
                // handle create avatar failed.
                if (cachedAvatarGameObject != null)
                {
                    Destroy(cachedAvatarGameObject);
                }

                throw;
            }
            finally
            {
                isCreatingAvatar = false;

                loadingIcon.SetActive(false);
            }

            return cachedAvatarGameObject != null;
        }

        private void StartAgent()
        {
            if (agentAvatar != null)
            {
                agentAvatar.Initialize(cachedAvatarGameObject);
                agentAvatar.Run();
            }
        }

        private void StopAgent()
        {
            if (agentAvatar != null)
            {
                agentAvatar.Shutdown();
            }
        }
    }
}