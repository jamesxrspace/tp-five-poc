using UnityEngine;
using UnityEngine.EventSystems;

namespace TPFive.Game.Profile.Test
{
    public class StringRemover : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private StringRepo stringRepo;

        public void OnPointerClick(PointerEventData eventData)
        {
            stringRepo.Remove();
        }
    }
}
