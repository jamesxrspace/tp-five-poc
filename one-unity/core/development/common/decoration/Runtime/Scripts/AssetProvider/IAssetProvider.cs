using System.Threading;
using Cysharp.Threading.Tasks;

namespace TPFive.Extended.Decoration
{
    /// <summary>
    /// AssetProvider interface is define the method for loading and releasing asset's instance.
    /// </summary>
    /// <typeparam name="T">Any type you want.</typeparam>
    public interface IAssetProvider<T>
    {
        // Load asset bundle into memory for instantiate faster.
        UniTask<T> LoadAsset(CancellationToken token = default);

        // Unload asset bundle from memory.
        UniTask UnloadAsset(CancellationToken token = default);

        // Load asset and create the instance.
        UniTask<T> CreateInstance(CancellationToken token = default);

        // Release the instantiated instance.
        UniTask<bool> ReleaseInstance(T target, CancellationToken token = default);
    }
}