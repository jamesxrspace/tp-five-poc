using System;
using Cysharp.Threading.Tasks;
using TPFive.Game.Avatar.Tracking;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager : MonoBehaviour
    {
        private IHumanPoseSynchronizer humanPoseSynchronizer;

        public IHumanPoseSynchronizer HumanPoseSynchronizer
        {
            get
            {
                if (humanPoseSynchronizer != null)
                {
                    return humanPoseSynchronizer;
                }

                humanPoseSynchronizer = userPlayer.Avatar.HumanPoseSynchronizer;

                return humanPoseSynchronizer ?? throw new ArgumentException(nameof(HumanPoseSynchronizer));
            }
        }

        public void PlayGeneratedMotion(byte[][] bufferList)
        {
            if (!machine.IsInState(Mode.Recording))
            {
                throw new InvalidOperationException($"Cannot play generated motion in state: {machine.State}");
            }

            // TODO: use the motion manager to access dummy avatar muscle data
            musicToMotionService.PlayAigcMotion(bufferList);
            musicToMotionService.OnMotionFinish += OnMotionFinish;
        }

        public void StopGeneratedMotion()
        {
            HumanPoseSynchronizer.Enabled = false;
            musicToMotionService.DestroyMotionPlayer();
            musicToMotionService.OnMotionFinish -= OnMotionFinish;
        }

        private void OnMotionFinish()
        {
            HumanPoseSynchronizer.Enabled = false;
            musicToMotionService.OnMotionFinish -= OnMotionFinish;
        }
    }
}
