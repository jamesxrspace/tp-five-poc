using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.UI
{
    public abstract class WindowBase : Window
    {
        [SerializeField]
        private UIRootType rootType;

        public UIRootType RootType => rootType;
    }
}
