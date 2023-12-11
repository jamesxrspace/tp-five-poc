using System;
using System.Collections.Generic;
using System.Linq;
using Mediapipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace TPFive.Game.Mocap
{
    using TPFive.Game;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using XR.BodyTracking;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Mocap.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private static readonly int NullServiceProviderIndex = (int)ServiceProviderKind.NullServiceProvider;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly XRAvatarAIMotionService _avatarAIMotionServicePrefab;
        private readonly MocapServiceSettings _mocapServiceSettings;
        private readonly FaceBlendShapeHandler _faceBlendShapeHandler;
        private readonly Dictionary<BodyPart, BodyPartInfo> _bodyPartInfoDict;

        private XRAvatarAIMotionService _avatarAIMotionService;
        private AIMotionDummyAvatarController _dummyAvatar;
        private HumanPoseHandler _humanPoseHandler;
        private HumanPose _capturedHumanPose;
        private SkinnedMeshRenderer _faceSkinnedMeshRenderer;
        private Mesh _faceMesh;
        private bool _isMocapEnabled;
        private bool _isSystemPreparing;
        private CaptureOptions _currentOptions;
        private ENUM_FaceTrackingType _activeFaceTrackingType;
        private bool _isFaceTrackingStarted;
        private bool _isBodyTrackingStarted;

        [Inject]
        public Service(
            MocapServiceSettings mocapServiceSettings,
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);
            _serviceProviderTable.Add(
                NullServiceProviderIndex,
                new NullServiceProvider((s, args) => Logger.LogDebug(s, args)));

            _mocapServiceSettings = mocapServiceSettings;
            _avatarAIMotionServicePrefab = mocapServiceSettings.AvatarAIMotionServicePrefab;
            _faceBlendShapeHandler = new FaceBlendShapeHandler();
            _bodyPartInfoDict = ((BodyPart[])Enum.GetValues(typeof(BodyPart)))
                .Select(x => new BodyPartInfo(x))
                .ToDictionary(x => x.BodyPart, x => x);

            ObservableExtensions.Subscribe(Observable.EveryUpdate(), Tick)
                .AddTo(_compositeDisposable);
        }

        public event MocapEnabledEventHandler OnMocapEnabled;

        public event MocapDisabledEventHandler OnMocapDisabled;

        public event Action OnFaceTrackingStarted;

        public event Action OnBodyTrackingStarted;

        public event TrackingStatusChangedEventHandler OnTrackingStatusChanged;

        public HumanPose CapturedHumanPose => _capturedHumanPose;

        public IFaceBlendShapeProvider FaceBlendShapeProvider => _faceBlendShapeHandler;

        public bool IsMocapEnabled => _isMocapEnabled;

        public IServiceProvider NullServiceProvider => _serviceProviderTable[NullServiceProviderIndex] as IServiceProvider;

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public TrackingStatus GetTrackingStatus(BodyPart bodyPart)
        {
            if (_bodyPartInfoDict.TryGetValue(bodyPart, out var info))
            {
                return info.Status;
            }

            return TrackingStatus.Loss;
        }

        public void EnableMocap(CaptureOptions options)
        {
            if (_isMocapEnabled)
            {
                return;
            }

            _isMocapEnabled = true;
            _isSystemPreparing = true;
            _isFaceTrackingStarted = true;
            _isBodyTrackingStarted = true;
            _currentOptions = options;
            _activeFaceTrackingType = GetFaceTrckingType(_currentOptions.IsEnableFace);

            var dummyAvatarGO = Object.Instantiate(_mocapServiceSettings.DummyAvatarPrefab);
            _dummyAvatar = dummyAvatarGO.GetComponent<AIMotionDummyAvatarController>();
            _humanPoseHandler = new HumanPoseHandler(_dummyAvatar.Animator.avatar, _dummyAvatar.AvatarModel.transform);
            _faceSkinnedMeshRenderer = _dummyAvatar.FaceSkinnedMeshRenderer;
            _faceMesh = _faceSkinnedMeshRenderer.sharedMesh;

            try
            {
                _faceBlendShapeHandler.MakeIndexMapping(_faceMesh);
            }
            catch (MismatchBlendShapeException e)
            {
                Logger.LogError(e.ToString());
            }

            _avatarAIMotionService = Object.Instantiate(_avatarAIMotionServicePrefab);
            var eventSource = _avatarAIMotionService.GetComponent<MonoBehaviourEventSource>();
            eventSource.OnEvent.AddListener(e =>
            {
                if (e == MonoBehaviourEventSource.Event.OnDestroy)
                {
                    DisableMocap();
                }
            });

            RegisterEvents();

            _avatarAIMotionService.InitSystemForInstantiate(_dummyAvatar.AvatarModel, null);
            _avatarAIMotionService.gameObject.SetActive(true);
        }

        public void DisableMocap()
        {
            if (!_isMocapEnabled)
            {
                return;
            }

            _isMocapEnabled = false;
            _isFaceTrackingStarted = false;
            _isBodyTrackingStarted = false;

            UnregisterEvents();

            if (_avatarAIMotionService)
            {
                _avatarAIMotionService.Stop();
                Object.Destroy(_avatarAIMotionService.gameObject);
            }

            if (_dummyAvatar)
            {
                Object.Destroy(_dummyAvatar.gameObject);
            }

            _isMocapEnabled = false;
            _currentOptions = default;
            OnMocapDisabled?.Invoke();
        }

        private void Tick(long tick)
        {
            if (!_isMocapEnabled || _isSystemPreparing)
            {
                return;
            }

            _humanPoseHandler.GetHumanPose(ref _capturedHumanPose);

            if (_activeFaceTrackingType == ENUM_FaceTrackingType.None ||
                _faceSkinnedMeshRenderer == null ||
                _faceMesh == null)
            {
                return;
            }

            _faceBlendShapeHandler.UpdateBlendShapeValues(_faceSkinnedMeshRenderer);
        }

        private void RegisterEvents()
        {
            if (_avatarAIMotionService == null)
            {
                return;
            }

            foreach (var info in _bodyPartInfoDict.Values)
            {
                var settings = _mocapServiceSettings.GetBodyPartSettings(info.BodyPart);
                info.CreateDetector(settings.LostTrackingThreshold);
                info.OnStatusChanged += OnBodyPartTrackingStatusChanged;
            }

            _avatarAIMotionService.OnPrepareSetting += OnPrepareSetting;
            _avatarAIMotionService.OnSystemReady += OnSystemReady;

            RegisterTrackingOutputEvents();
        }

        private void RegisterTrackingOutputEvents()
        {
            var detectorsToRegister = new List<ITrackingStatusDetector>();

            if (_currentOptions.IsEnableBody)
            {
                detectorsToRegister.AddRange(_bodyPartInfoDict
                    .Where(x => x.Key == BodyPart.Body || x.Key == BodyPart.LeftHand || x.Key == BodyPart.RightHand)
                    .Select(x => x.Value.Detector));
                _avatarAIMotionService.MediaPipe.OnPoseWorldOutput += OnBodyTrackingDataReceived;
            }

            if (_currentOptions.IsEnableFace)
            {
                if (_activeFaceTrackingType == ENUM_FaceTrackingType.Mediapipe)
                {
                    _avatarAIMotionService.MediaPipe.OnFaceBlendShapesOutput += OnFaceTrackingDataReceived;
                    detectorsToRegister.Add(_bodyPartInfoDict[BodyPart.Face].Detector);
                }
                else if (_activeFaceTrackingType == ENUM_FaceTrackingType.ARKits)
                {
                    _avatarAIMotionService.ARKitsFace.OnARKitsFaceBlendShapesOutput += OnFaceTrackingDataReceived;
                    _avatarAIMotionService.ARKitsFace.RegisterTrackingDetector(new ITrackingStatusDetector[] { _bodyPartInfoDict[BodyPart.Face].Detector });
                }
            }

            if (detectorsToRegister.Any())
            {
                _avatarAIMotionService.MediaPipe.RegisterTrackingDetector(detectorsToRegister.ToArray());
            }
        }

        private void UnregisterTrackingOutputEvents()
        {
            if (_currentOptions.IsEnableBody)
            {
                _avatarAIMotionService.MediaPipe.UnregisterTrackingDetector();
                _avatarAIMotionService.MediaPipe.OnPoseWorldOutput -= OnBodyTrackingDataReceived;
            }

            if (_currentOptions.IsEnableFace)
            {
                if (_activeFaceTrackingType == ENUM_FaceTrackingType.Mediapipe)
                {
                    _avatarAIMotionService.MediaPipe.OnFaceBlendShapesOutput -= OnFaceTrackingDataReceived;
                    _avatarAIMotionService.MediaPipe.UnregisterTrackingDetector();
                }
                else if (_activeFaceTrackingType == ENUM_FaceTrackingType.ARKits)
                {
                    _avatarAIMotionService.ARKitsFace.OnARKitsFaceBlendShapesOutput -= OnFaceTrackingDataReceived;
                    _avatarAIMotionService.ARKitsFace.UnregisterTrackingDetector();
                }
            }
        }

        private void OnFaceTrackingDataReceived(bool isSuccess, float[] blendShapeValues)
        {
            if (_isFaceTrackingStarted)
            {
                _isFaceTrackingStarted = false;
                OnFaceTrackingStarted?.Invoke();
            }
        }

        private void OnFaceTrackingDataReceived(bool isSuccess, ClassificationList classificationList)
        {
            if (_isFaceTrackingStarted)
            {
                _isFaceTrackingStarted = false;
                OnFaceTrackingStarted?.Invoke();
            }
        }

        private void OnBodyTrackingDataReceived(bool isSuccess, LandmarkList landmarkList)
        {
            if (_isBodyTrackingStarted)
            {
                _isBodyTrackingStarted = false;
                OnBodyTrackingStarted?.Invoke();
            }
        }

        private void OnBodyPartTrackingStatusChanged(BodyPart bodyPart, TrackingStatus status)
        {
            OnTrackingStatusChanged?.Invoke(bodyPart, status);
        }

        private void UnregisterEvents()
        {
            if (_avatarAIMotionService == null)
            {
                return;
            }

            _avatarAIMotionService.OnPrepareSetting -= OnPrepareSetting;
            _avatarAIMotionService.OnSystemReady -= OnSystemReady;
            foreach (var info in _bodyPartInfoDict.Values)
            {
                info.OnStatusChanged -= OnBodyPartTrackingStatusChanged;
            }

            UnregisterTrackingOutputEvents();
        }

        private void OnPrepareSetting(bool ready)
        {
            BodyTrackingSettings.TrackSourceType = (int)_mocapServiceSettings.TrackingSourceType;
            BodyTrackingSettings.FilterOneEuro = _mocapServiceSettings.FilterOneEuro;
            BodyTrackingSettings.FullBodyTracking = _currentOptions.IsEnableFullBody;
            BodyTrackingSettings.NeckTracking = _currentOptions.IsEnableFace;
            BodyTrackingSettings.FaceTrackingType = (int)_activeFaceTrackingType;
            BodyTrackingSettings.FaceBlendShapesScale = _mocapServiceSettings.FaceBlendShapesScale;
            BodyTrackingSettings.ZMoveScale = _mocapServiceSettings.ZMoveScale;
            BodyTrackingSettings.MoveRange = _mocapServiceSettings.MoveRange;
            BodyTrackingSettings.HumanPoseCorrection = _mocapServiceSettings.HumanPoseCorrection;
            BodyTrackingSettings.PoseCorrectionThreshold = _mocapServiceSettings.PoseCorrectionThreshold;
            BodyTrackingSettings.HandTrackingType = (int)_mocapServiceSettings.HandTrackingType;
            BodyTrackingSettings.BodyTrackingType = (int)_mocapServiceSettings.FullBodyTrackingType;
        }

        private void OnSystemReady(bool ready)
        {
            _isSystemPreparing = false;

            OnMocapEnabled?.Invoke(_currentOptions);
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                UnregisterEvents();

                _compositeDisposable?.Dispose();
                _humanPoseHandler?.Dispose();

                _humanPoseHandler = null;

                if (_avatarAIMotionService)
                {
                    Object.Destroy(_avatarAIMotionService.gameObject);
                }

                if (_dummyAvatar)
                {
                    Object.Destroy(_dummyAvatar.gameObject);
                }
            }

            _disposed = true;
        }

        private ENUM_FaceTrackingType GetFaceTrckingType(bool enabled)
        {
            if (!enabled)
            {
                return ENUM_FaceTrackingType.None;
            }

            return GameApp.Platform == Platform.iOS ? ENUM_FaceTrackingType.ARKits : ENUM_FaceTrackingType.Mediapipe;
        }

        private sealed class BodyPartInfo
        {
            private readonly BodyPart bodyPart;
            private TrackingStatus status;

            public BodyPartInfo(BodyPart bodyPart)
            {
                this.bodyPart = bodyPart;
                this.status = TrackingStatus.Loss;
            }

            public event TrackingStatusChangedEventHandler OnStatusChanged;

            public BodyPart BodyPart => bodyPart;

            public ITrackingStatusDetector Detector
            {
                get;
                private set;
            }

            public TrackingStatus Status
            {
                get => status;
                set
                {
                    if (status == value)
                    {
                        return;
                    }

                    status = value;
                    OnStatusChanged?.Invoke(bodyPart, status);
                }
            }

            public void CreateDetector(float lostTrackingThreshold)
            {
                Detector = new SingleTrackingStatusDetector(bodyPart, lostTrackingThreshold);
                Detector.OnTrackingStatusChanged += OnTrackingStatusChanged;
            }

            private void OnTrackingStatusChanged(TrackingStatus status)
            {
                Status = status;
            }
        }
    }
}