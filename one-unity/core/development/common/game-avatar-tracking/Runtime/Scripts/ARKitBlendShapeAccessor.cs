using TPFive.Game.Mocap;
using UnityEngine;

namespace TPFive.Game.Avatar.Tracking
{
    public class ARKitBlendShapeAccessor
    {
        private readonly SkinnedMeshRenderer skinnedMeshRenderer;
        private readonly int[] indexMapping;

        public ARKitBlendShapeAccessor(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            this.skinnedMeshRenderer = skinnedMeshRenderer != null
                ? skinnedMeshRenderer
                : throw new System.ArgumentNullException(nameof(skinnedMeshRenderer));

            indexMapping = new int[System.Enum.GetValues(typeof(ARKitBlendShapeLocation)).Length];
            MakeIndexMapping(skinnedMeshRenderer.sharedMesh);
        }

        public float GetBlendShapeWeight(ARKitBlendShapeLocation location)
        {
            var index = indexMapping[(int)location];
            return index >= 0 ? skinnedMeshRenderer.GetBlendShapeWeight(index) : 0f;
        }

        public void SetBlendShapeWeight(ARKitBlendShapeLocation location, float weight)
        {
            var index = indexMapping[(int)location];
            if (index >= 0)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
            }
        }

        private void MakeIndexMapping(Mesh mesh)
        {
            if (mesh == null)
            {
                throw new System.ArgumentNullException(nameof(mesh));
            }

            System.Array.Fill(indexMapping, -1);

            for (int i = 0, count = mesh.blendShapeCount; i < count; ++i)
            {
                var name = mesh.GetBlendShapeName(i);

                if (System.Enum.TryParse(name, true, out ARKitBlendShapeLocation location))
                {
                    indexMapping[(int)location] = i;
                }
            }
        }
    }
}