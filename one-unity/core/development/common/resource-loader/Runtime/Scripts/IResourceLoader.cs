using System;

namespace TPFive.Extended.ResourceLoader
{
    public interface IResourceLoader : IDisposable
    {
        string Url { get; }

        void Load();

        void Abort();

        object GetResource();
    }
}