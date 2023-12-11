using TPFive.SCG.ServiceEco.Abstractions;
using UniRx;
using VContainer;

namespace TPFive.Extended.InputDeviceProvider.OnScreen
{
    [RegisterToContainer]
    public partial class Service : IService
    {
        [Inject]
        public Service()
        {
            MoveStickController = new ReactiveProperty<IOnScreenStickController>();
            RotateStickController = new ReactiveProperty<IOnScreenStickController>();
        }

        public IReactiveProperty<IOnScreenStickController> MoveStickController { get; }

        public IReactiveProperty<IOnScreenStickController> RotateStickController { get; }
    }
}
