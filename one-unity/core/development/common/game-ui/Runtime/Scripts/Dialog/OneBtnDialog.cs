using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;

namespace TPFive.Game.UI
{
    public class OneBtnDialog : WindowBase
    {
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private UnityEngine.UI.Button confirmButton;
        [SerializeField]
        private TextMeshProUGUI buttonText;
        private OneBtnDialogViewModel viewModel;
        private Action clickAction;

        public Action ClickAction
        {
            get => clickAction;
            set => clickAction = value;
        }

        protected override void OnCreate(IBundle bundle)
        {
            var description = bundle.Get<object>("description", null) as string;
            var confirmText = bundle.Get<object>("confirmText", null) as string;

            viewModel = new OneBtnDialogViewModel(description, confirmText);
            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(descriptionText).For(v => v.text).To(vm => vm.Description);
            bindingSet.Bind(buttonText).For(v => v.text).To(vm => vm.ConfirmText);
            bindingSet.Bind(confirmButton).For(v => v.onClick).To(vm => vm.ConfirmCommand);
            bindingSet.Bind().For(v => v.ClickRequest).To(vm => vm.ConfirmRequest);
            bindingSet.Build();
        }

        private void ClickRequest(object sender, InteractionEventArgs e)
        {
            clickAction?.Invoke();
            Dismiss();
        }
    }
}
