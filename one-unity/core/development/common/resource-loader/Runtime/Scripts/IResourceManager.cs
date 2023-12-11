using System;

namespace TPFive.Extended.ResourceLoader
{
    public interface IResourceManager
    {
        void OnLoaderFinished(IResourceLoader loader);

        void OnLoaderFailed(IResourceLoader loader);
    }
}
