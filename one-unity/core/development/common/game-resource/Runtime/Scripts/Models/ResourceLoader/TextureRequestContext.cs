namespace TPFive.Game.Resource
{
    public class TextureRequestContext
        : ResourceRequestContext
    {
        public bool ApplyConversion { get; set; }

        public bool Fallback { get; set; }

        public ImageFormat NormalImageFormat { get; set; }

        // There may be the case where fallback need info to do some correction?
        public ImageFormat FallbackImageFormat { get; set; }

        // Add this to support error handling for texture loader.
        // It should be move to Response.
        public TextureDataErrorHandler ErrorHandler { get; set; }

        public override string ToString()
        {
            return $"url: {Url} applyConversion: {ApplyConversion} fallback: {Fallback}";
        }
    }
}
