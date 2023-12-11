using RenderHeads.Media.AVProVideo;
using TPFive.Game.Utils;
using UnityEngine;

namespace TPFive.Extended.Video.AVPro.Extension
{
    public static class ApplyToMeshExtensions
    {
        /// <summary>
        /// Apply Letterbox size in this view.
        /// The reason why Tilling and Scale of AVPro.ApplyToMesh is used is because it is considered that
        /// after this component has been decoded according to videos on different platforms,
        /// the image needs to be flipped Y-axis and inverted.
        /// Therefore, it will update the Tilling and Scale of the Material during Runtime.
        /// Therefore, we should not control the settings of the material on another component.
        /// Instead, we should set the Tilling and Scale properties provided by the component itself.
        /// </summary>
        /// <param name="applyToMesh">Your AVPro.ApplyToMesh component.</param>
        /// <param name="contentSize">Your content(Texture) size.</param>
        /// <param name="mappingSize">Your mapping(Screen) size.</param>
        public static void ApplyLetterbox(this ApplyToMesh applyToMesh, Vector2 contentSize, Vector2 mappingSize)
        {
            var uv = ViewportUtility.TransformViewportToUV(contentSize, mappingSize);

            var tiling = uv[0];
            var offset = uv[1];

            // Scale object UVs to fit texture
            applyToMesh.Scale = tiling;
            applyToMesh.Offset = offset;
        }
    }
}