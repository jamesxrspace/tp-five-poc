using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game
{
    public abstract class ServiceBase : IServiceProviderManagement
    {
#pragma warning disable SA1401
        protected readonly Dictionary<int, Game.IServiceProvider> _serviceProviderTable =
            new Dictionary<int, IServiceProvider>();
#pragma warning restore SA1401

        public IServiceProvider GetNullServiceProvider { get; }

        public async Task AddServiceProvider(int priority, IServiceProvider serviceProvider)
        {
            _serviceProviderTable.TryAdd(priority, serviceProvider);
        }

        public async Task RemoveServiceProvider(int priority)
        {
            var contained = _serviceProviderTable.ContainsKey(priority);
            if (contained)
            {
                _serviceProviderTable.Remove(priority);
            }
        }
    }
}
