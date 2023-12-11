using UnityEngine;
using UnityEngine.Events;

namespace TPFive.Home.Entry.SocialLobby
{
    public class PanEventDispatcher : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onPan;
        [SerializeField]
        private UnityEvent onPanEnd;
        [SerializeField]
        private float panEndCoolDown = 1f;
        private float panEndCoolDownTimer;
        private Vector3 lastPosition;

        public UnityEvent OnPan => onPan;

        public UnityEvent OnPanEnd => onPanEnd;

        protected void Update()
        {
            if (lastPosition != transform.position)
            {
                lastPosition = transform.position;
                OnPan?.Invoke();

                panEndCoolDownTimer = panEndCoolDown;
            }
            else if (panEndCoolDownTimer > 0)
            {
                // pan end
                panEndCoolDownTimer -= Time.deltaTime;
                if (panEndCoolDownTimer <= 0)
                {
                    OnPanEnd?.Invoke();
                }
            }
        }
    }
}