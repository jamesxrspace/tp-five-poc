using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Avatar.Timeline.AvatarObjectControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using XRSpace.PlayableMotion;

namespace TPFive.Creator.MotionConvertTool
{
    /// <summary>
    /// Convert <see cref="AvatarAttachObjectAsset"/> to <see cref="AvatarControlPlayableAsset"/>
    /// </summary>
    public class ControlPlayableCopy : PlayableCopy
    {
        public override void Copy(TimelineClip sourceClip, TimelineClip targetClip, TrackData trackData)
        {
            base.Copy(sourceClip, targetClip, trackData);
            var sourceVariables = GetSourceVariables((AvatarAttachObjectAsset)sourceClip.asset);
            SetTargetVariables((AvatarControlPlayableAsset)targetClip.asset, sourceVariables);
        }

        /// <summary>
        /// Get and convert asset properties from <see cref="AvatarAttachObjectAsset"/> to <see cref="ControlVariables"/>
        /// </summary>
        private ControlVariables GetSourceVariables(AvatarAttachObjectAsset asset)
        {
            var serializedObject = new SerializedObject(asset);
            var prefab = serializedObject.FindProperty("m_Prefab").objectReferenceValue as GameObject;
            var anchor = (AvatarAnchor)serializedObject.FindProperty("m_Anchor").intValue;
            var position = serializedObject.FindProperty("m_LocalPosition").vector3Value;
            var rotation = serializedObject.FindProperty("m_LocalRotaion").vector3Value;
            var scale = serializedObject.FindProperty("m_LocalScale").vector3Value;

            var targetAnchor = anchor switch
            {
                AvatarAnchor.Root => AnchorPointType.Root,
                AvatarAnchor.Hair => AnchorPointType.Hair,
                AvatarAnchor.Glasses => AnchorPointType.Glasses,
                AvatarAnchor.LeftEar => AnchorPointType.LeftEar,
                AvatarAnchor.RightEar => AnchorPointType.RightEar,
                AvatarAnchor.LeftWrist => AnchorPointType.LeftWrist,
                AvatarAnchor.RightWrist => AnchorPointType.RightWrist,
                AvatarAnchor.LeftPalm => AnchorPointType.LeftPalm,
                AvatarAnchor.RightPalm => AnchorPointType.RightPalm,
                _ => AnchorPointType.Root
            };

            return new ControlVariables
            {
                Prefab = prefab,
                Anchor = targetAnchor,
                LocalPosition = position,
                LocalRotation = rotation,
                LocalScale = scale,
            };
        }

        /// <summary>
        /// Set the properties of <see cref="AvatarControlPlayableAsset"/> from <see cref="ControlVariables"/>
        /// </summary>
        private void SetTargetVariables(AvatarControlPlayableAsset asset, ControlVariables variables)
        {
            var serializedObject = new SerializedObject(asset);
            var prefabProperty = serializedObject.FindProperty("_prefabGameObject");
            var anchorProperty = serializedObject.FindProperty("_avatarAnchor");
            var positionProperty = serializedObject.FindProperty("_localPosition");
            var rotationProperty = serializedObject.FindProperty("_localRotation");
            var scaleProperty = serializedObject.FindProperty("_localScale");
            prefabProperty.objectReferenceValue = variables.Prefab;
            anchorProperty.intValue = (int)variables.Anchor;
            positionProperty.vector3Value = variables.LocalPosition;
            rotationProperty.vector3Value = variables.LocalRotation;
            scaleProperty.vector3Value = variables.LocalScale;
            serializedObject.ApplyModifiedProperties();
        }

        private struct ControlVariables
        {
            public GameObject Prefab;
            public AnchorPointType Anchor;
            public Vector3 LocalPosition;
            public Vector3 LocalRotation;
            public Vector3 LocalScale;
        }
    }
}