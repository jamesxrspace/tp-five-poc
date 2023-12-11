using System;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;

namespace TPFive.Game.Decoration
{
    public sealed class DecorationCategoryItemViewModel : ViewModelBase
    {
        private readonly string categoryId;
        private readonly Action<DecorationCategoryItemViewModel> clickAction;
        private readonly SimpleCommand clickCommand;
        private readonly InteractionRequest clickRequest;
        private string categoryName;
        private int insidePointerNumber;
        private bool isOn;

        public DecorationCategoryItemViewModel(
            string categoryId,
            string categoryNameId,
            Action<DecorationCategoryItemViewModel> clickAction)
        {
            this.categoryId = categoryId;
            this.CategoryName = categoryNameId;
            this.clickAction = clickAction;

            clickCommand = new SimpleCommand(OnClick);
            clickRequest = new InteractionRequest(this);
        }

        public string CategoryId => categoryId;

        public ICommand ClickCommand => clickCommand;

        public IInteractionRequest ClickRequest => clickRequest;

        public string CategoryName
        {
            get => categoryName;
            set => Set(ref categoryName, value, nameof(CategoryName));
        }

        public int InsidePointerNumber
        {
            get => insidePointerNumber;
            set => Set(ref insidePointerNumber, Math.Max(value, 0), nameof(InsidePointerNumber));
        }

        public bool IsOn
        {
            get => isOn;
            set
            {
                if (Set(ref isOn, value, nameof(IsOn)))
                {
                    clickRequest.Raise();
                }
            }
        }

        private void OnClick()
        {
            IsOn = true;
            clickAction?.Invoke(this);
        }
    }
}
