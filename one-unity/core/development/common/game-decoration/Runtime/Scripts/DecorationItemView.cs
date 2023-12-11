using System;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.SpatialManipulation;
using TPFive.Extended.InputXREvent;
using TPFive.Game.Resource;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace TPFive.Game.Decoration
{
    public class DecorationItemView : UIView
    {
        [SerializeField]
        private RawImage image;
        [SerializeField]
        private ThumbnailLoader spriteLoader;
        [SerializeField]
        private DecorationObjectLoader objectLoader;

        private DecorationItemViewModel _viewModel;
        private Transform _handTransform;
        private MRTKRayInteractor _selectedInteractor;
        private GameObject _decorationObject;
        private IDisposable _pointerDownEvent;

        protected override void Start()
        {
            base.Start();
            _viewModel = this.GetDataContext() as DecorationItemViewModel;
            var bindingSet = this.CreateBindingSet(_viewModel);
            bindingSet.Bind(image).For(v => v.raycastTarget).To(vm => vm.IsVisible);
            bindingSet.Bind(image).For(v => v.enabled).To(vm => vm.IsVisible);
            bindingSet.Bind(spriteLoader).For(v => v.BundleID).To(vm => vm.BundleID);
            bindingSet.Bind(objectLoader).For(v => v.BundleID).To(vm => vm.BundleID);
            bindingSet.Build();
            AddEventTrigger();
        }

        protected void Update()
        {
            if (_selectedInteractor != null)
            {
                if (_selectedInteractor.SelectProgress < 0.5f)
                {
                    OnHandRelease();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _pointerDownEvent.Dispose();
            _viewModel = null;
        }

        private void AddEventTrigger()
        {
            _pointerDownEvent = ObservableExtensions.Subscribe(image.OnPointerDownAsObservable(), _ =>
            {
                if (HandshapeHelpers.TransmitterVR.LeftRayInteractor.TryGetCurrentUIRaycastResult(out var result))
                {
                    _selectedInteractor = HandshapeHelpers.TransmitterVR.LeftRayInteractor;
                    _handTransform = HandshapeHelpers.TransmitterVR.LeftHand;
                }
                else
                {
                    _selectedInteractor = HandshapeHelpers.TransmitterVR.RightRayInteractor;
                    _handTransform = HandshapeHelpers.TransmitterVR.RightHand;
                }

                SpawnDecorationObject();
            });
        }

        private void SpawnDecorationObject()
        {
            objectLoader.CreateInstance()
                .ContinueWith(go =>
                {
                    _decorationObject = go;
                    _decorationObject.transform.SetParent(_handTransform.transform, true);
                    _decorationObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    _decorationObject.AddComponent<ObjectManipulator>();
                }).Forget();
        }

        private void OnHandRelease()
        {
            _decorationObject.transform.SetParent(null);
            _handTransform = null;
            _selectedInteractor = null;
        }
    }
}