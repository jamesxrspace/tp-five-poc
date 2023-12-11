using System.Threading.Tasks;

namespace TPFive.Game
{
    public interface IServiceProviderManagement
    {
        IServiceProvider GetNullServiceProvider { get; }

        Task AddServiceProvider(int priority, IServiceProvider serviceProvider);

        Task RemoveServiceProvider(int priority);
    }

    public interface IServiceProvider
    {
    }
}
