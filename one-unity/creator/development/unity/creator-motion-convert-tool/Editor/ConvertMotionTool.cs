using System;
using System.IO;
using TPFive.Game.Avatar.Timeline.AvatarObjectControl;
using TPFive.Game.Avatar.TimelineMotion.TimeMachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using XRSpace.PlayableMotion;

namespace TPFive.Creator.MotionConvertTool
{
    public class ConvertMotionTool : EditorWindow
    {
        [SerializeField]
        private TimelineAsset[] timelineAssets = Array.Empty<TimelineAsset>();

        private float scrollViewHeight;
        private Vector2 scrollPosition;

        [MenuItem("Tools/ConvertMotionTool")]
        public static void ShowWindow()
        {
            GetWindow<ConvertMotionTool>();
        }

        protected void OnGUI()
        {
            // Create a scroll view to show the list of assets
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.MaxHeight(scrollViewHeight)))
            {
                scrollPosition = scrollViewScope.scrollPosition;

                var serializedObject = new SerializedObject(this);
                var assets = serializedObject.FindProperty("timelineAssets");
                EditorGUILayout.PropertyField(assets, true);
                serializedObject.ApplyModifiedProperties();
                scrollViewHeight = EditorGUI.GetPropertyHeight(assets);
            }

            GUILayout.Label(string.Empty, GUILayout.Height(10f));

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            // The button to start the convert task.
            if (GUILayout.Button("Go", GUILayout.Width(100f), GUILayout.Height(50f)))
            {
                if (timelineAssets == null)
                {
                    Debug.LogError("TimelineAssets is null.");
                    return;
                }

                var directoryPath = Path.Combine(Application.dataPath, "Resources", "ConvertMotions");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                AssetDatabase.StartAssetEditing();

                foreach (var asset in timelineAssets)
                {
                    if (asset == null)
                    {
                        Debug.LogWarning("Some timeline asset is null.");
                        continue;
                    }

                    var timelineAsset = CreateInstance<TimelineAsset>();

                    var newPath = Path.Combine("Assets", "Resources", "ConvertMotions", $"{asset.name}.playable");

                    Debug.Log(newPath);

                    AssetDatabase.CreateAsset(timelineAsset, newPath);

                    CopyMotionAsset(asset, timelineAsset);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.StopAssetEditing();
                EditorUtility.RequestScriptReload();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        private void CopyMotionAsset(TimelineAsset sourceAsset, TimelineAsset targetAsset)
        {
            var tracks = sourceAsset.GetOutputTracks();

            // Create tracks
            var trackData = new TrackData
            {
                TimeMachineTrack = targetAsset.CreateTrack<TimeMachineTrack>(),
                AnimationTrack = targetAsset.CreateTrack<AnimationTrack>(),
                AudioTrack = targetAsset.CreateTrack<AudioTrack>(),
                AvatarControlTrack = targetAsset.CreateTrack<AvatarControlTrack>(),
            };

            // Copy clips
            foreach (var track in tracks)
            {
                foreach (var timelineClip in track.GetClips())
                {
                    CopyClip(timelineClip, trackData);
                }
            }

            // Remove empty track
            foreach (var track in targetAsset.GetOutputTracks())
            {
                if (!track.hasClips)
                {
                    targetAsset.DeleteTrack(track);
                }
            }
        }

        private void CopyClip(TimelineClip timelineClip, TrackData trackData)
        {
            TimelineClip newClip = null;
            PlayableCopy playableCopy = null;

            if (timelineClip.asset is AvatarPlayAnimationAsset animationAsset)
            {
                newClip = animationAsset.AnimationClip == null
                    ? trackData.AnimationTrack.CreateClip<AnimationPlayableAsset>()
                    : trackData.AnimationTrack.CreateClip(animationAsset.AnimationClip);

                playableCopy = new AnimationPlayableCopy();
            }
            else if (timelineClip.asset is AvatarAttachObjectAsset)
            {
                newClip = trackData.AvatarControlTrack.CreateClip<AvatarControlPlayableAsset>();

                playableCopy = new ControlPlayableCopy();
            }
            else if (timelineClip.asset is AudioPlayableAsset)
            {
                newClip = trackData.AudioTrack.CreateClip<AudioPlayableAsset>();

                playableCopy = new AudioPlayableCopy();
            }

            if (newClip != null)
            {
                playableCopy.Copy(timelineClip, newClip, trackData);
            }
        }
    }
}