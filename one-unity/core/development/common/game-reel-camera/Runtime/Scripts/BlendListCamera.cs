using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    public class BlendListCamera : BaseReelCamera
    {
        public override void Play(Transform target)
        {
            base.Play(target);
            gameObject.SetActive(true);

            // CinemachineBlendListCamera will auto play
        }
    }
}
