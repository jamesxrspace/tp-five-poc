using System;
using System.Threading;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Motion;
using UnityEngine;
using VContainer;

namespace TPFive.Creator.MotionConvertTool
{
    /// <summary>
    /// This is a demo script to play motion at avatar.
    /// </summary>
    public class MotionDemo : MonoBehaviour
    {
        [Inject]
        private readonly IAvatarFactory factory;

        [SerializeField]
        private string avatarFormatJson;

        private bool isLoading;

        private GameObject avatarRoot;

        private IAvatarMotionManager motionManager;

        private string motionUid = Guid.Empty.ToString();

        protected async void OnGUI()
        {
            if (isLoading)
            {
                return;
            }

            if (avatarRoot == null)
            {
                avatarFormatJson = GUILayout.TextArea(avatarFormatJson, GUILayout.Width(300));

                if (GUILayout.Button("Create Avatar"))
                {
                    isLoading = true;
                    CreateAvatar(() => { isLoading = false; });
                }

                return;
            }

            if (GUILayout.Button("Delete Avatar"))
            {
                Destroy(avatarRoot);
            }

            GUILayout.BeginHorizontal();

            var blackTextStyle = new GUIStyle(GUI.skin.label) { normal = { textColor = Color.black } };

            // change to black color
            GUILayout.Label("Motion Uid : ", blackTextStyle);

            motionUid = GUILayout.TextField(motionUid, GUILayout.Width(300));

            GUILayout.EndHorizontal();

            if (Guid.TryParse(motionUid, out var uid) == false)
            {
                return;
            }

            if (GUILayout.Button("Play"))
            {
                motionManager.Play(uid);
                motionManager.Weight = 1;
            }

            if (GUILayout.Button("Stop"))
            {
                motionManager.Stop();
            }
        }

        private async void CreateAvatar(Action callback)
        {
            var (avatarFormat, error) = AvatarFormat.Deserialize(avatarFormatJson);
            var (result, avatar) = await factory.Create(avatarFormat, CancellationToken.None);

            if (!result)
            {
                Debug.LogError("Create avatar failed.");
                callback?.Invoke();
                return;
            }

            avatarRoot = avatar;
            var context = avatar.GetComponentInChildren<AvatarContextProvider>();
            motionManager = context.MotionManager;

            callback?.Invoke();
        }
    }
}