using System;
using TPFive.Game.Record.Scene;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public class SessionStartOption
    {
        private bool enableRecord;

        /// <summary>
        /// Gets or sets the start mode indicating how the session starts.
        /// </summary>
        public ReelState State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user's avatar would be loaded into the scene.
        /// </summary>
        public bool JoinUserAvatar { get; set; }

        public bool EnableRecord => EnableRecordMotion || EnableRecordVoice;

        /// <summary>
        /// Gets or sets a value indicating whether start record avatar's motion when session start.
        /// </summary>
        public bool EnableRecordMotion
        {
            get => enableRecord && JoinUserAvatar;
            set => enableRecord = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether record voice when session start.
        /// </summary>
        public bool EnableRecordVoice { get; set; }

        /// <summary>
        /// Gets or sets background music.
        /// </summary>
        public AudioClip BgmClip { get; set; }

        /// <summary>
        /// Gets a value indicating whether enable background music.
        /// </summary>
        public bool EnableBgm => BgmClip != null;

        /// <summary>
        /// Gets a value indicating whether to play music-generated motion after the session starts.
        /// </summary>
        public bool EnableMusicToMotion => MusicMotion != null && JoinUserAvatar;

        /// <summary>
        /// Gets or sets the music-generated motion data to play.
        /// </summary>
        public byte[][] MusicMotion { get; set; }

        /// <summary>
        /// Gets or sets session duration in seconds.
        /// </summary>
        public int Duration { get; set; } = 30;

        public Action<bool> PlaybackFinishedHandler { get; set; }

        public static SessionStartOption Watch()
        {
            return new SessionStartOption
            {
                State = ReelState.Watch,
            };
        }
    }
}