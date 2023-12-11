using Loxodon.Framework.Binding;
using UnityEngine;
using UnityEngine.Assertions;

namespace TPFive.Home.Entry.SocialLobby
{
    public class PlayerEntityViewMobile : PlayerEntityViewBase
    {
        [SerializeField]
        private EntityManipulatorMobile entityManipulator;

        public override void Initialize(PlayerEntityViewModel dataContext)
        {
            base.Initialize(dataContext);

            entityManipulator.HostTransform = dataContext.EntityManipulationTransform;
            entityManipulator.ClickThreshold = dataContext.InputSetting.ClickThreshold;
            entityManipulator.Rigidbody = dataContext.EntityManipulationRigidbody;
            Assert.IsTrue(
                entityManipulator.ClickThreshold > 0,
                $"entityManipulator.ClickDelay should great than 0, but is {entityManipulator.ClickThreshold}");

            entityManipulator.OnEntityClicked.AddListener(OnEntityClicked);
        }

        protected override void OnDestroy()
        {
            if (entityManipulator != null)
            {
                entityManipulator.OnEntityClicked.RemoveListener(OnEntityClicked);
            }

            base.OnDestroy();
        }

        private void OnEntityClicked()
        {
            if (this.GetDataContext() is PlayerEntityViewModel viewModel)
            {
                viewModel.GotoReelSceneCommand.Execute(null);
            }
        }
    }
}