using System;
using UnityEngine;

namespace TPFive.Game.Record.Scene
{
    /// <summary>
    /// Provides information about the reel scene told runtime how to present.<br/>
    /// e.g. use which camera for specific seat, what reel camera tags be carry ...etc.
    /// </summary>
    [Serializable]
    public sealed class ReelSceneInfo
    {
        [SerializeField]
        [Tooltip("The setting of browse state.")]
        private ReelStateSetting watchReelSetting = new ReelStateSetting(ReelState.Watch);

        [SerializeField]
        [Tooltip("Whether enable prepare state. Some case may not need prepare state. e.g. Carpool.")]
        private bool enablePrepareState;

        [SerializeField]
        [Tooltip("The setting of preparing state.")]
        private ReelStateSetting prepareRecordSetting = new ReelStateSetting(ReelState.Prepare);

        [SerializeField]
        [Tooltip("The setting of standby state.")]
        private ReelStateSetting standByRecordSetting = new ReelStateSetting(ReelState.Standby);

        [SerializeField]
        [Tooltip("The setting of recording state.")]
        private ReelStateSetting recordingSetting = new ReelStateSetting(ReelState.Recording);

        [SerializeField]
        [Tooltip("The setting of preview state.")]
        private ReelStateSetting previewRecordSetting = new ReelStateSetting(ReelState.Preview);

        [SerializeField]
        [Tooltip("Get random track when record reel video.")]
        private bool randomTrack;

        [SerializeField]
        [Tooltip("Enable music to motion button.")]
        private bool enableMusicToMotion;

        [SerializeField]
        [Tooltip("Camera Target Type of reel tracks' initial position.")]
        private ReelCameraTargetType reelCameraTargetType;

        [SerializeField]
        [Tooltip("Camera track initial position.")]
        private Vector3 fixedPosition;

        public bool EnablePrepareState => enablePrepareState;

        public bool RandomTrack => randomTrack;

        public bool EnableMusicToMotion => enableMusicToMotion;

        public ReelCameraTargetType ReelCameraTargetType => reelCameraTargetType;

        public Vector3 FixedPosition => fixedPosition;

        public ReelStateSetting GetSettingByState(ReelState state)
        {
            var result = state switch
            {
                ReelState.Watch => watchReelSetting,
                ReelState.Prepare => prepareRecordSetting,
                ReelState.Standby => standByRecordSetting,
                ReelState.Recording => recordingSetting,
                ReelState.Preview => previewRecordSetting,
                _ => throw new NotImplementedException($"Unknown state({state})")
            };

            if (!result.AlignState.HasValue)
            {
                return result;
            }

            return GetSettingByState(result.AlignState.Value);
        }
    }
}
