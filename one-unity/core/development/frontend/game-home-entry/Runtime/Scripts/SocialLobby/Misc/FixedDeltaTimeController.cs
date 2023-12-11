using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class FixedDeltaTimeController : MonoBehaviour
    {
        [SerializeField]
        private float fixedDeltaTime = 0.033f;

        private float originalFixedDeltaTime;

        protected void OnEnable()
        {
            originalFixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = fixedDeltaTime;
        }

        protected void OnDisable()
        {
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }
}