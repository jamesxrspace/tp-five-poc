using UnityEngine;

namespace TPFive.Extended.InputXREvent
{
    [CreateAssetMenu(fileName = "InputSettings", menuName = "TPFive/Extended/InputXREvent/Input Settings")]
    public class InputSettings : ScriptableObject
    {
        [SerializeField]
        private float waitingBufferTime;
        [SerializeField]
        private float clickThreshold;
        [SerializeField]
        private float longPressThreshold;

        public float WaitingBufferTime => waitingBufferTime;

        public float ClickThreshold => clickThreshold;

        public float LongPressThreshold => longPressThreshold;
    }
}