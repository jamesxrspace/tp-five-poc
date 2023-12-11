using UnityEngine;

namespace TPFive.Extended.Addressable
{
    public abstract class AssetLocatorBase<TKey, TLocation>
        : ScriptableObject,
        IAssetLocator<TKey, TLocation>
    {
        public abstract TLocation GetLocation(TKey key);
    }
}
