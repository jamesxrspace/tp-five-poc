#if UNITY_EDITOR
using TPFive.Game.Utils;
using TPFive.Game.Video;
using UnityEngine;

namespace TPFive.Extended.Video.AVPro
{
    using UnityEditor;

    public sealed partial class VideoPlayer
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private bool IsTestApplyToMeshComponent
        {
            get => EditorPrefs.GetBool(nameof(IsTestApplyToMeshComponent), true);
            set => EditorPrefs.SetBool(nameof(IsTestApplyToMeshComponent), value);
        }

        private Renderer RenderTarget => applyToMesh.MeshRenderer;

        [ContextMenu("Test/" + nameof(EnableTestApplyToMeshComponent))]
        private void EnableTestApplyToMeshComponent()
        {
            IsTestApplyToMeshComponent = true;
        }

        [ContextMenu("Test/" + nameof(DisableTestApplyToMeshComponent))]
        private void DisableTestApplyToMeshComponent()
        {
            IsTestApplyToMeshComponent = false;
        }

        [ContextMenu("Test/" + nameof(TestPortraitSize))]
        private void TestPortraitSize()
        {
            ApplyLetterbox(
                RenderTarget,
                fromSize: new Vector2(9, 16),
                toSize: RenderTarget.transform.localScale);
        }

        [ContextMenu("Test/" + nameof(TestLandScapeSize))]
        private void TestLandScapeSize()
        {
            ApplyLetterbox(
                RenderTarget,
                fromSize: new Vector2(16, 9),
                toSize: RenderTarget.transform.localScale);
        }

        [ContextMenu("Test/" + nameof(TestSquareSize))]
        private void TestSquareSize()
        {
            ApplyLetterbox(
                RenderTarget,
                fromSize: Vector2.one,
                toSize: RenderTarget.transform.localScale);
        }

        [ContextMenu("Test/" + nameof(TestFitScreen))]
        private void TestFitScreen()
        {
            ApplyLetterbox(
                RenderTarget,
                fromSize: Vector2.one,
                toSize: Vector2.one);
        }

        [ContextMenu("Test/" + nameof(TestSetFitScreen))]
        private void TestSetFitScreen()
        {
            ContentDisplayType = ContentDisplayType.Fit;
        }

        [ContextMenu("Test/" + nameof(TestSetStretchScreen))]
        private void TestSetStretchScreen()
        {
            ContentDisplayType = ContentDisplayType.Stretch;
        }

        private void ApplyLetterbox(Renderer target, Vector2 fromSize, Vector2 toSize)
        {
            var uv = ViewportUtility.TransformViewportToUV(fromSize, toSize);

            var tiling = uv[0];
            var offset = uv[1];

            if (IsTestApplyToMeshComponent)
            {
                applyToMesh.Offset = offset;
                applyToMesh.Scale = tiling;
            }
            else
            {
                // Scale object UVs to fit texture
                target.material.SetTextureScale(MainTex, tiling);
                target.material.SetTextureOffset(MainTex, offset);
            }
        }
    }
}
#endif