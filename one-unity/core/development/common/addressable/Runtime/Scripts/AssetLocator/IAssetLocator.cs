namespace TPFive.Extended.Addressable
{
    public interface IAssetLocator<TKey, TLocation>
    {
        public TLocation GetLocation(TKey key);
    }
}
