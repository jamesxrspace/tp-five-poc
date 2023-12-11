using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.Events;

namespace TPFive.Game.UI
{
    [RequireComponent(typeof(Slider))]
    public sealed class ForwardSliderEvent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<float> onValueChanged;

        [SerializeField]
        private Slider slider;

        private void Start()
        {
            if (slider == null)
            {
                slider = GetComponent<Slider>();
            }

            if (slider != null)
            {
                slider.OnValueUpdated.AddListener(OnValueUpdated);
            }
        }

        private void OnDestroy()
        {
            if (slider != null)
            {
                slider.OnValueUpdated.RemoveListener(OnValueUpdated);
            }
        }

        private void OnValueUpdated(SliderEventData e)
        {
            onValueChanged.Invoke(e.NewValue);
        }
    }
}
