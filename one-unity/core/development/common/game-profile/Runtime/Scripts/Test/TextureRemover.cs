using UnityEngine;
using UnityEngine.EventSystems;

namespace TPFive.Game.Profile.Test
{
    public class TextureRemover : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private TextureRepo textureRepo;

        public void OnPointerClick(PointerEventData eventData)
        {
            textureRepo.Remove();
        }
    }
}
