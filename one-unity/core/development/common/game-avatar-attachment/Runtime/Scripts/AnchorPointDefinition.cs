using System;
using UnityEngine;

namespace TPFive.Game.Avatar.Attachment
{
    [Serializable]
    public class AnchorPointDefinition
    {
        [SerializeField]
        private AnchorPointType type;

        [SerializeField]
        private HumanBodyBones parentBone;

        // The offset from HumanBodyBones to the attach point.
        [SerializeField]
        private Vector3 offset;

        // The rotation from HumanBodyBones to the attach point.
        [SerializeField]
        private Vector3 rotation;

        public AnchorPointType Type => type;

        public HumanBodyBones ParentBone => parentBone;

        public Vector3 Offset => offset;

        public Vector3 Rotation => rotation;

        public static string TypeToGameObjectName(AnchorPointType type)
        {
            return $"AnchorPoint - {type}";
        }

        public string GetTypeGameObjectName()
        {
            return TypeToGameObjectName(type);
        }
    }
}