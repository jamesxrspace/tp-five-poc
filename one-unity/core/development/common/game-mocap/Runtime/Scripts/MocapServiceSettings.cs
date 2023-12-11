using System;
using System.Collections.Generic;
using UnityEngine;
using XR.BodyTracking;

namespace TPFive.Game.Mocap
{
    [Serializable]
    public struct BodyPartSettings
    {
        [SerializeField]
        private BodyPart bodyPart;
        [SerializeField]
        private float lostTrackingThreshold;

        public readonly BodyPart BodyPart => bodyPart;

        public readonly float LostTrackingThreshold => lostTrackingThreshold;
    }

    [CreateAssetMenu(fileName = "Mocap Service Settings", menuName = "TPFive/Game/Mocap Service Settings")]
    public class MocapServiceSettings : ScriptableObject
    {
        [SerializeField]
        private XRAvatarAIMotionService avatarAIMotionServicePrefab;
        [SerializeField]
        private GameObject dummyAvatarPrefab;
        [Header("Body Tracking Settings")]
        [SerializeField]
        private ENUM_TrackingSourceType trackingSourceType = ENUM_TrackingSourceType.WebCam;
        [SerializeField]
        private bool filterOneEuro = false;
        [SerializeField]
        private int faceBlendShapesScale = 100;
        [SerializeField]
        private float zMoveScale = 2f;
        [SerializeField]
        private Rect moveRange = new (3f, 1.5f, 6f, 2f);
        [SerializeField]
        private bool humanPoseCorrection = false;
        [SerializeField]
        private int poseCorrectionThreshold = 25;
        [SerializeField]
        private ENUM_HandTrackingType handTrackingType = ENUM_HandTrackingType.Mediapipe;
        [SerializeField]
        private ENUM_BodyTrackingType fullBodyTrackingType = ENUM_BodyTrackingType.Mediapipe;
        [SerializeField]
        private BodyPartSettings[] bodyPartSettings;

        public XRAvatarAIMotionService AvatarAIMotionServicePrefab => avatarAIMotionServicePrefab;

        public GameObject DummyAvatarPrefab => dummyAvatarPrefab;

        public ENUM_TrackingSourceType TrackingSourceType => trackingSourceType;

        public bool FilterOneEuro => filterOneEuro;

        public int FaceBlendShapesScale => faceBlendShapesScale;

        public float ZMoveScale => zMoveScale;

        public Rect MoveRange => moveRange;

        public bool HumanPoseCorrection => humanPoseCorrection;

        public int PoseCorrectionThreshold => poseCorrectionThreshold;

        public ENUM_HandTrackingType HandTrackingType => handTrackingType;

        public ENUM_BodyTrackingType FullBodyTrackingType => fullBodyTrackingType;

        public BodyPartSettings GetBodyPartSettings(BodyPart bodyPart)
        {
            return Array.Find(bodyPartSettings, x => x.BodyPart == bodyPart);
        }
    }
}