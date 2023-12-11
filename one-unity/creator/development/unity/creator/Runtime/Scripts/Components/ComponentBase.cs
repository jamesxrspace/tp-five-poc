using System;
using UniRx;
using UnityEngine;

namespace TPFive.Creator
{
    public abstract class ComponentBase
#if XSPO_NETWORKING
        : NetworkBehaviour
#else
        : MonoBehaviour
#endif
    {
        protected CompositeDisposable CompositeDisposable { get; set; } = new CompositeDisposable();

        protected virtual void OnDestroy()
        {
            CompositeDisposable?.Dispose();
        }
    }
}
