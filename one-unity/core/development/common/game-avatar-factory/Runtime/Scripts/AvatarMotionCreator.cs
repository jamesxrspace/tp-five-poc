using Animancer;
using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Avatar.Motion;
using TPFive.Game.Extensions;
using UnityEngine;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Factory
{
    /// <summary>
    /// This creator is used to set the require components of avatar timeline,
    /// and set the avatar motion assets.
    /// </summary>
    public static class AvatarMotionCreator
    {
        public static IAvatarMotionManager Create(
            GameObject root,
            AnimancerComponent animancer,
            IAnchorPointProvider provider,
            AvatarMotionCategory category)
        {
            // Set up Timeline
            var playableDirector = root.GetOrAddComponent<PlayableDirector>();
            playableDirector.enabled = false;
            playableDirector.playOnAwake = false;

            // Set up AudioSource
            var audioSource = root.GetOrAddComponent<AudioSource>();

            // Set up AvatarMotionManager
            var motionManager = root.GetOrAddComponent<AvatarMotionManager>();
            motionManager.Animancer = animancer;
            motionManager.AnimancerLayerIndex = AvatarUtility.GetLayerIndex(AvatarAnimancerLayer.Motion);
            motionManager.AudioSource = audioSource;
            motionManager.AnchorPointProvider = provider;
            motionManager.SetAvatarMotionCategory(category);

            return motionManager;
        }
    }
}