using System;
using UnityEngine;
using XR.BodyTracking;

namespace TPFive.Game.Mocap
{
    public delegate void MocapEnabledEventHandler(CaptureOptions options);

    public delegate void MocapDisabledEventHandler();

    public delegate void TrackingStatusChangedEventHandler(BodyPart bodyPart, TrackingStatus status);

    public interface IService
    {
        event MocapEnabledEventHandler OnMocapEnabled;

        event MocapDisabledEventHandler OnMocapDisabled;

        event Action OnFaceTrackingStarted;

        event Action OnBodyTrackingStarted;

        event TrackingStatusChangedEventHandler OnTrackingStatusChanged;

        IServiceProvider NullServiceProvider { get; }

        HumanPose CapturedHumanPose { get; }

        IFaceBlendShapeProvider FaceBlendShapeProvider { get; }

        bool IsMocapEnabled { get; }

        TrackingStatus GetTrackingStatus(BodyPart bodyPart);

        void EnableMocap(CaptureOptions options);

        void DisableMocap();
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        event MocapEnabledEventHandler OnMocapEnabled;

        event MocapDisabledEventHandler OnMocapDisabled;

        event Action OnFaceTrackingStarted;

        event Action OnBodyTrackingStarted;

        event TrackingStatusChangedEventHandler OnTrackingStatusChanged;

        HumanPose CapturedHumanPose { get; }

        IFaceBlendShapeProvider FaceBlendShapeProvider { get; }

        bool IsMocapEnabled { get; }

        XR.BodyTracking.TrackingStatus GetTrackingStatus(XR.BodyTracking.BodyPart bodyPart);

        void EnableMocap(CaptureOptions options);

        void DisableMocap();
    }
}
