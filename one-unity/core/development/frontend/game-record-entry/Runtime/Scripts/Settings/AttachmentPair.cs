using System;
using UnityEngine;

namespace TPFive.Game.Record.Entry.Settings
{
    // Define the attachment how to pair Attachment(Decoration) category and AvatarMotionMode.
    [Serializable]
    public class AttachmentPair
    {
        [SerializeField]
        private ReelAvatarMotionMode avatarMotionMode;

        [SerializeField]
        private string category;

        public ReelAvatarMotionMode AvatarMotionMode => avatarMotionMode;

        public string Category => category;
    }
}