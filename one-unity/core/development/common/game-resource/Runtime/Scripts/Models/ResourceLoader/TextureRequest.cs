namespace TPFive.Game.Resource
{
    public sealed class TextureRequest : ResourceRequest<TextureData>
    {
        /// <summary>
        /// callback : This callback is defined and used by texture loader while asset finished loading.
        /// You should check result exists or not before using it.
        /// </summary>
        public TextureRequest(object owner, TextureRequestContext context, OnRequestFinishedDelegate<TextureData> callback)
            : base(owner, context, callback)
        {
        }
    }
}
