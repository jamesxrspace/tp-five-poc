using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject.
    /// It also allows for scrolling by restricting panning to one direction.
    /// </summary>
    [AddComponentMenu("Scripts/SocialLobby/PanTexture")]
    public class PanTexture : MonoBehaviour
    {
        [SerializeField]
        private Renderer textureRenderer = null;
        [SerializeField]
        private Vector2 panScale = Vector2.one;

        private bool IsValid => textureRenderer != null && textureRenderer.enabled;

        public void ApplyPanToTarget(Vector2 panOffset)
        {
            if (IsValid)
            {
                var offset = new Vector2(panOffset.x * panScale.x, panOffset.y * panScale.y);
                textureRenderer.materials[0].mainTextureOffset += offset; // Pan
            }
        }

        protected void OnValidate()
        {
            if (textureRenderer == null)
            {
                textureRenderer = GetComponent<Renderer>();
            }
        }
    }
}