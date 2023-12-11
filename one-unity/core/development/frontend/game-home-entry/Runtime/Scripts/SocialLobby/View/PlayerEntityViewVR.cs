using Loxodon.Framework.Binding;
using TPFive.Extended.InputXREvent;
using UnityEngine;
using XRInputData = TPFive.Extended.InputXREvent.XRInputData;

namespace TPFive.Home.Entry.SocialLobby
{
    public class PlayerEntityViewVR : PlayerEntityViewBase
    {
        [SerializeField]
        private XREventInteractable eventInteractable;
        [SerializeField]
        private string mockReelId = "1e7d3803-954c-44c7-aeab-401e8c93f7c9";
        private PlayerEntityViewModel _viewModel;

        public override void Initialize(PlayerEntityViewModel dataContext)
        {
            base.Initialize(dataContext);
            _viewModel = this.GetDataContext() as PlayerEntityViewModel;
            SetMockData();
            eventInteractable.AddListener(XRInputDataEvent.EventType.Click, OnEntityClicked);
        }

        public void OnEntityClicked(XRInputData inputData)
        {
            if (this.GetDataContext() is PlayerEntityViewModel viewModel)
            {
                viewModel.GotoReelSceneCommand.Execute(null);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            eventInteractable.RemoveListener(XRInputDataEvent.EventType.Click, OnEntityClicked);
        }

        private void SetMockData()
        {
            _viewModel.ReelId = mockReelId;
        }
    }
}