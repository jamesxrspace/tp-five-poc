using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;

namespace TPFive.Game.Home.Entry
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly SimpleCommand<bool>[] selectCommands;
        private readonly InteractionRequest<int> selectedRequest;
        private readonly InteractionRequest<int> deselectedRequest;
        private ILogger logger;

        public MainMenuViewModel(ILoggerFactory loggerFactory, int itemCount)
        {
            logger = loggerFactory.CreateLogger<MainMenuViewModel>();

            selectedRequest = new InteractionRequest<int>(this);
            deselectedRequest = new InteractionRequest<int>(this);
            selectCommands = new SimpleCommand<bool>[itemCount];
            for (int i = 0; i < selectCommands.Length; ++i)
            {
                var index = i;
                selectCommands[i] = new SimpleCommand<bool>(isOn =>
                {
                    if (isOn)
                    {
                        OnSelectionChanged(index);
                    }
                    else
                    {
                        OnDeselectionChanged(index);
                    }
                });
            }
        }

        public ICommand[] SelectCommands => selectCommands;

        public InteractionRequest<int> SelectedRequest => selectedRequest;

        public InteractionRequest<int> DeselectedRequest => deselectedRequest;

        private void OnSelectionChanged(int index)
        {
            selectedRequest.Raise(index);
        }

        private void OnDeselectionChanged(int index)
        {
            deselectedRequest.Raise(index);
        }
    }
}
