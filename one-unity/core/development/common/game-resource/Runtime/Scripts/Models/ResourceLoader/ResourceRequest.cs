using System;

namespace TPFive.Game.Resource
{
    public delegate void OnRequestFinishedDelegate<T>(T response);

    public class ResourceRequest<T>
        where T : IDisposable
    {
        private readonly object owner;
        private readonly OnRequestFinishedDelegate<T> callback;
        private ResourceRequestContext _resourceRequestContext;

        public ResourceRequest(
            object owner,
            ResourceRequestContext resourceRequestContext,
            OnRequestFinishedDelegate<T> callback)
        {
            this.owner = owner;
            _resourceRequestContext = resourceRequestContext;
            this.callback = callback;
        }

        public string Url => _resourceRequestContext.Url;

        public ResourceRequestContext Context => _resourceRequestContext;

        public void DispatchResource(ResourceReferenceInfo<T> resourceInfo)
        {
            // Get resource and set reference from resource info
            T resource = resourceInfo.Require(this.owner);

            // Dispatch resource
            this.callback?.Invoke(resource);
        }
    }
}
