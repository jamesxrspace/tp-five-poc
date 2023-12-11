using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TPFive.Game.Resource
{
    public class ResourceReferenceInfo<T>
        where T : IDisposable
    {
        private readonly string resourceUrl;
        private T resource;
        private List<WeakReference> references = new List<WeakReference>();

        public ResourceReferenceInfo(string resourceUrl, T resource)
        {
            this.resourceUrl = resourceUrl;
            this.resource = resource;
        }

        public bool IsUnused => UpdateReference() == 0;

        private ILogger Logger { get; set; }

        public T Require(object user)
        {
            this.Retain(user);
            return resource;
        }

        public void Release(object owner)
        {
            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].Target == owner)
                {
                    references.RemoveAt(i);
                    break;
                }
            }
        }

        public void UnloadResource()
        {
            if (!IsUnused)
            {
                Logger?.LogDebug($"Force unload resource {resourceUrl}. " +
                    $"(Unreleased reference owner: {string.Join(", ", references.Select(x => x.Target.GetType().Name))})");
            }

            resource?.Dispose();
        }

        private void Retain(object owner)
        {
            if (owner == null)
            {
                throw new Exception("Please set the user!");
            }

            for (int i = 0; i < references.Count; i++)
            {
                var target = references[i].Target;
                if (owner.Equals(target))
                {
                    return;
                }
            }

            WeakReference wr = new WeakReference(owner);
            references.Add(wr);
        }

        private int UpdateReference()
        {
            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].Target == null)
                {
                    references.RemoveAt(i);
                    i--;
                }
            }

            return references.Count;
        }
    }
}