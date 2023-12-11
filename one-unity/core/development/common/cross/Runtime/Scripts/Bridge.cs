#pragma warning disable SA1401
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Cross
{
    /// <summary>
    /// This is the bridge used to cross the boundary between main and creator projects.
    /// </summary>
    public sealed partial class Bridge
    {
        public static ShowChatbotDialogueDelegate ShowChatbotDialogue;
        public static ShowMessageBoardWindowDelegate ShowMessageBoardWindow;
        public static ShowMultiButtonDialogueDelegate ShowMultiButtonDialogue;
        public static ShowRedeemDialogueDelegate ShowRedeemDialogue;
        public static ShowToastDelegate ShowToast;
        public static ShowTutorialDialogueDelegate ShowTutorialDialogue;
        public static ShowVoiceMessageBoardWindowDelegate ShowVoiceMessageBoardWindow;
        public static ShowVoiceRecordingWindowDelegate ShowVoiceRecordingWindow;
        public static TeleportActorDelegate TeleportActor;
        public static MoveActorToDelegate MoveActorTo;
        public static PublishMessageDelegate PublishMessage;
        public static PublishAnalyticsMessageDelegate PublishAnalyticsMessage;
        public static IsLocalHostDelegate IsLocalHost;
        public static IsOwnerDelegate IsOwner;

        public delegate void ShowChatbotDialogueDelegate(string name);

        public delegate void ShowMessageBoardWindowDelegate(string name);

        public delegate void ShowMultiButtonDialogueDelegate(string name);

        public delegate void ShowRedeemDialogueDelegate(string name);

        public delegate void ShowToastDelegate(string name);

        public delegate void ShowTutorialDialogueDelegate(string name);

        public delegate void ShowVoiceMessageBoardWindowDelegate(string name);

        public delegate void ShowVoiceRecordingWindowDelegate(string name);

        public delegate bool TeleportActorDelegate(GameObject actor, Vector3 position);

        public delegate void MoveActorToDelegate(GameObject actor, Vector3 position);

        public delegate void PublishMessageDelegate(string name, string stringParam);

        public delegate void PublishAnalyticsMessageDelegate(string name);

        public delegate bool IsLocalHostDelegate();

        public delegate bool IsOwnerDelegate();
    }
}
#pragma warning restore SA1401
