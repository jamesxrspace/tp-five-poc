using TPFive.Game.Resource;
using UnityEngine;

namespace TPFive.Extended.ResourceLoader
{
    public sealed class TextureManager : ResourceManager<TextureData>
    {
        [SerializeField]
        private int maxLoadingAmount = 4;

        protected override int MAX_LOADING_AMOUNT => maxLoadingAmount;

        public void SetMaxLoadingAmount(int amount)
        {
            maxLoadingAmount = amount;

            // After MAX_LOADING_AMOUNT changed, check loader queue to make waiting loader work
            ProcessPendingQueue();
        }
    }
}
