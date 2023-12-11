using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby.Misc
{
    public class FixedTransform : MonoBehaviour
    {
        [SerializeField]
        private bool fixedWorldSpacePosition;
        [SerializeField]
        private bool fixedWorldSpaceRotation;
        [SerializeField]
        private bool fixedLocalSpaceScale;
        [SerializeField]
        private Vector3 targetWorldSpacePosition = Vector3.zero;
        [SerializeField]
        private Quaternion targetWorldSpaceRotation = Quaternion.identity;
        [SerializeField]
        private Vector3 targetLocalSpaceScale = Vector3.one;

        protected void OnEnable()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;

                if (fixedWorldSpacePosition)
                {
                    transform.position = targetWorldSpacePosition;
                }

                if (fixedWorldSpaceRotation)
                {
                     transform.rotation = targetWorldSpaceRotation;
                }

                if (fixedLocalSpaceScale)
                {
                    transform.localScale = targetLocalSpaceScale;
                }
            }
        }
    }
}