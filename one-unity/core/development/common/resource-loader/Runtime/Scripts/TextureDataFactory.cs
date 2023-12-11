namespace TPFive.Extended.ResourceLoader
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Resource;

    public class TextureDataFactory
    {
        private ILoggerFactory loggerFactory;
        private ILogger<TextureDataFactory> logger;

        public TextureDataFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public ILogger Logger => logger ??= loggerFactory.CreateLogger<TextureDataFactory>();

        // FIXME: remove pragma warning disable 1998 when Enable KTX is enabled
#pragma warning disable 1998
        public async Task<TextureData> Create(TextureDataCreationContext context)
#pragma warning restore 1998
        {
            try
            {
                if (context.Data == null)
                {
                    return TextureData.Default;
                }

                var textureData = TextureData.Default;
                if (context.ImageFormat == ImageFormat.Normal)
                {
                    textureData = new TextureData(context.Data);
                }

                textureData.OriginalWidth = context.Width;
                textureData.OriginalHeight = context.Height;
                textureData.HasOrientation = context.HasOrientation;
                textureData.Orientation = context.Orientation;

                return textureData;
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(TextureDataFactory)} create failed", e);
                return TextureData.Default;
            }
        }
    }
}