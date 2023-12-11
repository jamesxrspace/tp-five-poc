using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.SceneManagement;

namespace TPFive.Game.SceneFlow
{
    public sealed partial class Service
    {
        private async UniTask SetupMessageHandling(CancellationToken cancellationToken = default)
        {
            _subChangeScene
                .Subscribe(HandleChangeScene())
                .AddTo(_compositeDisposable);
        }

        private System.Action<ChangeScene> HandleChangeScene()
        {
            return async x =>
            {
                if (x.FromCategoryOrder != x.ToCategoryOrder)
                {
                    Logger.LogWarning(
                        "x.FromCategoryOrder {x.FromCategoryOrder} != x.ToCategoryOrder {x.ToCategoryOrder}",
                        x.FromCategoryOrder,
                        x.ToCategoryOrder);
                }

                var lifetimeScope = x.LifetimeScope.Parent != null ? x.LifetimeScope.Parent : x.LifetimeScope;
                await UnloadSceneAsync(x.FromTitle, x.FromCategoryOrder, x.FromSubOrder, x.FromScriptableObject);
                await LoadSceneAsync(
                    x.ToTitle,
                    LoadSceneMode.Additive,
                    x.ToCategoryOrder,
                    x.ToSubOrder,
                    lifetimeScope,
                    x.ToScriptableObject);
            };
        }
    }
}
