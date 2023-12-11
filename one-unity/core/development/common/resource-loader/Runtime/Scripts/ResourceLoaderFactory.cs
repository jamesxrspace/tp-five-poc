using Microsoft.Extensions.Logging;
using TPFive.Game.Resource;

namespace TPFive.Extended.ResourceLoader
{
    public class ResourceLoaderFactory
    {
        public static IResourceLoader Create<T>(
            IResourceManager resourceManager,
            ResourceRequestContext context,
            ILoggerFactory loggerFactory)
        {
            var type = typeof(T);

            if (type == typeof(TextureData) && context is TextureRequestContext textureRequestContext)
            {
                return CreateTextureLoader(resourceManager, textureRequestContext, loggerFactory);
            }
#if ENABLE_PDF
            if (type == typeof(PortableDocumentFormat))
            {
                return CreatePDFLoader(resourceManager, context);
            }
#endif

            return null;
        }

        private static IResourceLoader CreateTextureLoader(
            IResourceManager resourceManager,
            TextureRequestContext context,
            ILoggerFactory loggerFactory)
        {
            var pool = new RemoteResponsePool();
            var textureDataFactory = new TextureDataFactory(loggerFactory);

            pool.Setup();

            // Pass dependencies into constructor
            return new TextureLoader(
                resourceManager,
                context,
                pool,
                textureDataFactory,
                loggerFactory);
        }

#if ENABLE_PDF
        private static IResourceLoader CreatePDFLoader(IResourceManager resourceManager, ResourceRequestContext context)
        {
            return new PortableDocumentFormatLoader(resourceManager, context.Url);
        }
#endif
    }
}