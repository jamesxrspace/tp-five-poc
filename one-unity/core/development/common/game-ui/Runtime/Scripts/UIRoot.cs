using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.UI
{
#pragma warning disable SA1305 // Field names should not use Hungarian notation
    public sealed class UIRoot : MonoBehaviour
    {
        [SerializeField]
        private UIRootType type;
        [SerializeField]
        private MonoBehaviour windowManager;

        public UIRootType Type => type;

        public IWindowManager WindowManager => windowManager as IWindowManager;
    }
#pragma warning restore SA1305 // Field names should not use Hungarian notation
}
