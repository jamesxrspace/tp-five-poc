using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Mocap
{
    internal class FaceBlendShapeHandler : IFaceBlendShapeProvider
    {
        private readonly float[] blendShapeValues;
        private readonly int[] indexMapping;

        public FaceBlendShapeHandler()
        {
            blendShapeValues = new float[Enum.GetValues(typeof(ARKitBlendShapeLocation)).Length];
            indexMapping = new int[blendShapeValues.Length];
        }

        public float GetBlendShapeValue(ARKitBlendShapeLocation location)
        {
            return blendShapeValues[(int)location];
        }

        internal void MakeIndexMapping(Mesh faceMesh)
        {
            Array.Fill(indexMapping, -1);

            for (int i = 0, count = faceMesh.blendShapeCount; i < count; ++i)
            {
                var name = faceMesh.GetBlendShapeName(i);

                if (Enum.TryParse(name, true, out ARKitBlendShapeLocation location))
                {
                    indexMapping[(int)location] = i;
                }
            }

            var locations = new List<ARKitBlendShapeLocation>();
            for (var i = 0; i < indexMapping.Length; ++i)
            {
                if (indexMapping[i] < 0)
                {
                    locations.Add((ARKitBlendShapeLocation)i);
                }
            }

            if (locations.Count > 0)
            {
                throw new MismatchBlendShapeException(locations, "Mismatch blend shapes");
            }
        }

        internal void UpdateBlendShapeValues(SkinnedMeshRenderer renderer)
        {
            for (int i = 0; i < indexMapping.Length; ++i)
            {
                if (indexMapping[i] >= 0)
                {
                    blendShapeValues[i] = renderer.GetBlendShapeWeight(indexMapping[i]);
                }
                else
                {
                    blendShapeValues[i] = 0;
                }
            }
        }
    }
}
