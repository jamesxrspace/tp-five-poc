using UnityEngine;
using UnityEngine.EventSystems;

namespace TPFive.Game.Profile.Test
{
    public class StringGenerator : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private StringRepo stringRepo;

        public void OnPointerClick(PointerEventData eventData)
        {
            stringRepo.Add();
        }
    }
}
