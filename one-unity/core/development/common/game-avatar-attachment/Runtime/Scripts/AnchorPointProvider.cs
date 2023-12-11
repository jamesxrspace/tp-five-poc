using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPFive.Game.Avatar.Attachment
{
    /// <summary>
    /// This class assists in the creation and reference of avatar anchor transform.
    /// </summary>
    public class AvatarAnchorPointProvider : IAnchorPointProvider
    {
        private readonly IReadOnlyDictionary<AnchorPointType, Transform> anchorPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarAnchorPointProvider"/> class, which will create avatar anchor transform at avatar.
        /// </summary>
        /// <param name="avatarTransform"> The transform from avatar. </param>
        /// <param name="animator"> The animator from avatar. </param>
        /// <param name="anchors"> The avatar anchor settings from avatar. </param>
        public AvatarAnchorPointProvider(Transform avatarTransform, Animator animator, AnchorPointDefinition[] anchors)
        {
            anchorPoints = CreateAnchorPoints(avatarTransform, animator, anchors);
        }

        public bool TryGetAnchorPoint(AnchorPointType anchorType, out Transform anchorTransform)
        {
            return anchorPoints.TryGetValue(anchorType, out anchorTransform);
        }

        private static Dictionary<AnchorPointType, Transform> CreateAnchorPoints(
            Transform avatarTransform,
            Animator animator,
            AnchorPointDefinition[] definitions)
        {
            return definitions.Aggregate(
                new Dictionary<AnchorPointType, Transform>(definitions.Length + 1)
                {
                    { AnchorPointType.Root, avatarTransform },
                },
                (acc, current) =>
                {
                    if (current.Type == AnchorPointType.Root)
                    {
                        return acc;
                    }

                    if (acc.Keys.Contains(current.Type))
                    {
                        throw new Exception($"Duplicate anchor point type: {current.Type}");
                    }

                    var boneTransform = animator.GetBoneTransform(current.ParentBone);
                    if (boneTransform == null)
                    {
                        return acc;
                    }

                    var transform = new GameObject(current.GetTypeGameObjectName()).transform;
                    transform.SetParent(boneTransform);
                    transform.SetLocalPositionAndRotation(current.Offset, Quaternion.Euler(current.Rotation));

                    acc.Add(current.Type, transform);
                    return acc;
                });
        }
    }
}