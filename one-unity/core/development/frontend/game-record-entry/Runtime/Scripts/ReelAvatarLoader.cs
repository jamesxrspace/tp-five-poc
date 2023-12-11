using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.Decoration;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using AvatarAnchorPointType = TPFive.Game.Avatar.Attachment.AnchorPointType;
using DecorationAnchorPointType = TPFive.Game.Decoration.Attachment.AnchorPointType;
using IDecorationService = TPFive.Game.Decoration.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IUserService = TPFive.Game.User.IService;

namespace TPFive.Game.Record.Entry
{
    public class ReelAvatarLoader
    {
        private static readonly Vector3 DefaultPosition = new Vector3(0, 1000, 0);
        private readonly IAvatarFactory avatarFactory;
        private readonly IUserService userService;
        private readonly IDecorationService decorationService;
        private readonly ILogger logger;
        private readonly IObjectResolver objectResolver;
        private readonly GameObject playerPrefab;

        public ReelAvatarLoader(
            IAvatarFactory avatarFactory,
            IUserService userService,
            ILoggerFactory loggerFactory,
            IObjectResolver objectResolver,
            IDecorationService decorationService,
            GameObject playerPrefab = null)
        {
            this.avatarFactory = avatarFactory;
            this.userService = userService;
            this.objectResolver = objectResolver;
            this.playerPrefab = playerPrefab;
            this.decorationService = decorationService;

            logger = loggerFactory.CreateLogger<ReelAvatarLoader>();
        }

        public async UniTask<ReelPlayer> Load(AvatarFormat avatarFormat, CancellationToken cancellationToken)
        {
            GameObject player = null;

            try
            {
                if (playerPrefab != null)
                {
                    player = objectResolver.Instantiate(playerPrefab);
                    player.transform.position = DefaultPosition;
                    var avatar = player.transform.Find("Avatar");
                    _ = await avatarFactory.Setup(avatar.gameObject, avatarFormat, cancellationToken);
                }
                else
                {
                    var (_, go) = await avatarFactory.Create(avatarFormat, cancellationToken);
                    if (go == null)
                    {
                        throw new ReelException("Failed to load avatar from avatar format.");
                    }

                    go.transform.position = DefaultPosition;
                    player = go;
                }

                player.GetComponent<AvatarMovement>().CanRun = false;

                // Wait 2 frames for all components to be initialized.
                // Wait Awake() & OnEnable()
                // Wait Start() & Update()
                await UniTask.DelayFrame(2, cancellationToken: cancellationToken);

                player.GetComponent<AvatarLoader>().OnLoaded?.Invoke();
                SceneManager.MoveGameObjectToScene(player, SceneManager.GetActiveScene());
            }
            catch (OperationCanceledException)
            {
                if (player != null)
                {
                    UnityEngine.Object.Destroy(player);
                }

                throw;
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to load avatar from avatar format. {e}");
                throw;
            }

            return new ReelPlayer(player);
        }

        public UniTask<ReelPlayer> LoadUserPlayer(CancellationToken cancellationToken)
        {
            return userService.GetUserAsync()
                .ContinueWith(user => userService.GetAvatarProfile(user.Uid, cancellationToken))
                .ContinueWith(avatarProfile => Load(avatarProfile.Format, cancellationToken));
        }

        public async UniTask<DecorationAttachment> CreateDecorationAttachment(string groupId, string bundleId)
        {
            var decoration = await decorationService.InstantiateAsync(groupId, bundleId);
            if (decoration == null)
            {
                logger.LogWarning(
                    "{MethodName} fail : {GroupId} and {BundleId} is not found.",
                    nameof(CreateDecorationAttachment),
                    groupId,
                    bundleId);

                return null;
            }

            if (decoration.TryGetComponent<DecorationAttachment>(out var attachment))
            {
                return attachment;
            }

            logger.LogWarning(
                "{MethodName} fail : This decoration {GroupId} and {BundleId} without DecorationAttachment. Auto Destroy it.",
                nameof(CreateDecorationAttachment),
                groupId,
                bundleId);

            await decorationService.DestroyAsync(groupId, bundleId, decoration);

            return null;
        }

        public async UniTask<bool> DestroyDecorationAttachment(string groupId, string bundleId, DecorationAttachment attachment)
        {
            if (attachment == null)
            {
                logger.LogWarning("{MethodName} fail : DecorationAttachment is null.", nameof(DestroyDecorationAttachment));
                return false;
            }

            return await decorationService.DestroyAsync(groupId, bundleId, attachment.gameObject);
        }

        /// <summary>
        /// Try apply decoration to avatar anchor point.
        /// The definition of DecorationAttachment is setup from content project.
        /// </summary>
        /// <param name="attachment">This DecorationAttachment component is the loaded decoration gameObject.</param>
        /// <param name="category">The category of the loaded DecorationAttachment which define the offset and rotation and attach point type.</param>
        /// <param name="anchorPointProvider">The avatar anchor point look up table.</param>
        /// <returns></returns>
        public bool TryApplyAttachmentToAnchor(DecorationAttachment attachment, string category, IAnchorPointProvider anchorPointProvider)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            if (anchorPointProvider == null)
            {
                logger.LogWarning(
                    "{MethodName} fail : {Interface} is null. Category={Category}",
                    nameof(TryApplyAttachmentToAnchor),
                    nameof(IAnchorPointProvider),
                    category);

                return false;
            }

            if (!attachment.TryGetDefinition(category, out var definition))
            {
                logger.LogWarning("{MethodName} fail : Definition is not exit by {Category}", nameof(TryApplyAttachmentToAnchor), category);
                return false;
            }

            var anchorPointType = ConvertToAvatarAnchorPointType(definition.Type);

            if (!anchorPointProvider.TryGetAnchorPoint(
                    anchorPointType,
                    out var anchorTransform))
            {
                logger.LogWarning("{MethodName} fail : {AnchorPointType} is not found in avatar anchor point.", nameof(TryApplyAttachmentToAnchor), anchorPointType);
                return false;
            }

            attachment.transform.SetParent(anchorTransform, false);
            attachment.transform.SetLocalPositionAndRotation(definition.Offset, Quaternion.Euler(definition.Rotation));

            return true;
        }

        private static AvatarAnchorPointType ConvertToAvatarAnchorPointType(DecorationAnchorPointType type)
        {
            return type switch
            {
                DecorationAnchorPointType.Root => AvatarAnchorPointType.Root,
                DecorationAnchorPointType.RightWrist => AvatarAnchorPointType.RightWrist,
                DecorationAnchorPointType.LeftWrist => AvatarAnchorPointType.LeftWrist,
                DecorationAnchorPointType.Glasses => AvatarAnchorPointType.Glasses,
                DecorationAnchorPointType.LeftEar => AvatarAnchorPointType.LeftEar,
                DecorationAnchorPointType.RightEar => AvatarAnchorPointType.RightEar,
                DecorationAnchorPointType.LeftPalm => AvatarAnchorPointType.LeftPalm,
                DecorationAnchorPointType.RightPalm => AvatarAnchorPointType.RightPalm,
                DecorationAnchorPointType.Hair => AvatarAnchorPointType.Hair,
                DecorationAnchorPointType.Abdomen => AvatarAnchorPointType.Abdomen,
                _ => throw new NotImplementedException($"Without handle type={type}")
            };
        }
    }
}
