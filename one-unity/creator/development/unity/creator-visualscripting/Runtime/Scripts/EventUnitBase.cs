using UniRx;
using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting
{
    public abstract class EventUnitBase<T> : EventUnit<T>
    // public abstract class EventUnitBase<T> : GameObjectEventUnit<T>
    {
        protected readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
    }
}
