#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Utils
{
    public class GraphicRaycastTargetGizmos : MonoBehaviour
    {
        private static readonly int MaxCornerNum = 4;
        private static readonly Vector3[] FourCorners = new Vector3[MaxCornerNum];

        protected void Awake()
        {
            DontDestroyOnLoad(this);
        }

        protected void OnDrawGizmos()
        {
            var graphics = FindObjectsByType<MaskableGraphic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var graphic in graphics)
            {
                if (!graphic.raycastTarget)
                {
                    continue;
                }

                var rectTransform = graphic.transform as RectTransform;
                rectTransform.GetWorldCorners(FourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < MaxCornerNum; ++i)
                {
                    Gizmos.DrawLine(FourCorners[i], FourCorners[(i + 1) % MaxCornerNum]);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            _ = new GameObject($"{nameof(GraphicRaycastTargetGizmos)} - Editor Only", typeof(GraphicRaycastTargetGizmos));
        }
    }
}
#endif