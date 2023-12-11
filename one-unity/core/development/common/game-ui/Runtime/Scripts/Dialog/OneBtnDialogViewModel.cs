using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;

namespace TPFive.Game.UI
{
    public class OneBtnDialogViewModel : ViewModelBase
    {
        private string description;
        private string confirmText;
        private InteractionRequest confirmRequest;
        private SimpleCommand confirmCommand;

        public OneBtnDialogViewModel(string description, string confirmText)
        {
            Description = description;
            ConfirmText = confirmText;
            confirmRequest = new InteractionRequest(this);
            confirmCommand = new SimpleCommand(OnButtonClicked);
        }

        public string Description
        {
            get => description;
            set => Set(ref description, value, nameof(Description));
        }

        public string ConfirmText
        {
            get => confirmText;
            set => Set(ref confirmText, value, nameof(ConfirmText));
        }

        public ICommand ConfirmCommand => confirmCommand;

        public IInteractionRequest ConfirmRequest => confirmRequest;

        private void OnButtonClicked()
        {
            confirmRequest.Raise();
        }
    }
}
