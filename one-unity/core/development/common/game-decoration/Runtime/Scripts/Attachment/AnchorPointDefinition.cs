using System;
using UnityEngine;

namespace TPFive.Game.Decoration.Attachment
{
    [Serializable]
    public class AnchorPointDefinition
    {
        [SerializeField]
        private string category;

        [SerializeField]
        private AnchorPointType type;

        // The offset from HumanBodyBones to the attach point.
        [SerializeField]
        private Vector3 offset;

        // The rotation from HumanBodyBones to the attach point.
        [SerializeField]
        private Vector3 rotation;

        public string Category => category;

        public AnchorPointType Type => type;

        public Vector3 Offset => offset;

        public Vector3 Rotation => rotation;
    }
}