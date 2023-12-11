using System;
using System.Collections.Generic;
using System.Linq;
using TPFive.Game.Record.Scene;
using TPFive.Game.Reel.Camera;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager : MonoBehaviour
    {
        private const string HeadCountTag = "HeadCount";
        private Coroutine trackCoroutine;

        public void BuildDirector()
        {
            if (userPlayer == null)
            {
                throw new InvalidOperationException("User avatar is not loaded");
            }

            // TODO: Fix this when camera tag move to content
            var reelSceneTagString = reelSceneInfo.RandomTrack ? "Default" : "Carpool";

            var cameraTagList = new List<ReelCameraTag>()
            {
                reelDirectorConfig.GetReelCameraTag(reelSceneTagString),
                reelDirectorConfig.GetReelCameraTag(HeadCountTag + playlistPlayers.Count.ToString()),
            };

            Vector3 initialPosition = reelSceneInfo.ReelCameraTargetType switch
            {
                ReelCameraTargetType.FixedPosition => reelSceneInfo.FixedPosition,
                _ => sourceFootage.OfType<AvatarRecordData>().First().InitialPosition,
            };

            // LiveCamera must be FixedCamera
            reelDirector.Build(cameraTagList, initialPosition, cameraService.GetLiveCamera().Pose);
        }

        public void PlaySelectedCameraTrack(int index)
        {
            // Stop trackCoroutine make reelDirector not restoring internal state correctly,
            // we need to do it manually by calling RestoreContext, so that we can PlayTrack
            // again in a correct state.
            ResetCurrentTrack();

            var playerRoot = playlistPlayers[0].Root.transform;
            var cameraTarget = playerRoot.Find("Avatar/Camera Target");
            var maxDuration = sourceFootage.OfType<AvatarRecordData>().Select(x => x.GetLengthSec()).Max();
            trackCoroutine = StartCoroutine(reelDirector.PlayTrack(index, maxDuration, cameraTarget));
        }

        public int GetCameraTrackCount()
        {
            return reelDirector.CameraTrackCount;
        }

        private void ResetCurrentTrack()
        {
            if (trackCoroutine != null)
            {
                StopCoroutine(trackCoroutine);
                trackCoroutine = null;
                reelDirector.RestoreContext();
            }
        }
    }
}
