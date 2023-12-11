using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPFive.Home.Entry.SocialLobby
{
    [CreateAssetMenu(menuName = "SocialLobby/Layout/XZPlaneLayoutLogic", fileName = "XZPlaneLayoutLogic")]
    public class XZPlaneLayoutLogic : AbstractPlaneLayoutLogic
    {
        /// <summary>
        /// Determine the layout based on which axis.
        /// </summary>
        [SerializeField]
        private LayoutAxis mainLayoutAxis;
        [SerializeField]
        private float spacingX;
        [SerializeField]
        private float spacingZ;

        /// <summary>
        /// Each entity will have a margin to perform random position offset to perform nature placement.
        /// Don't use the same value when LayoutAxis = X to prevent entity overlapping.
        /// </summary>
        [SerializeField]
        private float randomOffsetX;

        /// <summary>
        /// Each entity will have a margin to perform random position offset to perform nature placement.
        /// Don't use the same value when LayoutAxis = Z to prevent entity overlapping.
        /// </summary>
        [SerializeField]
        private float randomOffsetZ;

        /// <summary>
        /// Use this value to offset the position of the alternate row, like chess board.
        /// </summary>
        [SerializeField]
        private float alternateOffsetX;

        /// <summary>
        /// Use this value to offset the position of the alternate column, like chess board.
        /// </summary>
        [SerializeField]
        private float alternateOffsetZ;

        public override LayoutAxis MainLayoutAxis => mainLayoutAxis;

        public override void PopulateFromStartPosition(List<IEntity> entities, Vector3 startLocalPosition)
        {
            entities.Sort(SortBySortOrder);

            var entityCenter2D = new Vector2(startLocalPosition.x, startLocalPosition.z);
            for (var index = 0; index < entities.Count; index++)
            {
                var entity = entities[index];

                // calculate center position
                // get model size 2D
                var entitySize2D = new Vector2(entity.Size.x, entity.Size.z);
                var halfEntitySize2D = entitySize2D / 2;
                var entityCenter2DBuffer = entityCenter2D + halfEntitySize2D;

                // random offset
                var randomX = Random.Range(0, randomOffsetX);
                var randomZ = Random.Range(0, randomOffsetZ);
                entityCenter2DBuffer += new Vector2(randomX, randomZ);

                // set position in 3D,
                // clamp y-axis to start position plane because it's xz plane layout.
                var entityCenter3D = new Vector3(entityCenter2DBuffer.x, startLocalPosition.y, entityCenter2DBuffer.y);

                // apply alternate offset for chess board layout
                var isApplyAlternateOffset = index % 2 != 0;
                if (isApplyAlternateOffset)
                {
                    entityCenter3D += new Vector3(alternateOffsetX, 0, alternateOffsetZ);
                }

                entity.LocalPosition = entityCenter3D;

                // calculate next position
                var spacing2D = new Vector2(spacingX, spacingZ);
                var nextEntityCenter2D = entityCenter2DBuffer + halfEntitySize2D + spacing2D;

                switch (mainLayoutAxis)
                {
                    case LayoutAxis.X:
                        nextEntityCenter2D = new Vector2(nextEntityCenter2D.x, entityCenter2D.y);
                        break;
                    case LayoutAxis.Z:
                        nextEntityCenter2D = new Vector2(entityCenter2D.x, nextEntityCenter2D.y);
                        break;
                    default:
                        throw new NotImplementedException(
                        $"{nameof(XZPlaneLayoutLogic)}: Not support layout axis {mainLayoutAxis} in this layout logic");
                }

                // prepare pos for next entity
                entityCenter2D = nextEntityCenter2D;
            }
        }

        private int SortBySortOrder(IEntity x, IEntity y)
        {
            return x.SortOrder.CompareTo(y.SortOrder);
        }
    }
}