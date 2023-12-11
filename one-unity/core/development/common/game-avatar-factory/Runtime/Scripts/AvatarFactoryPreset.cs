using Animancer;
using TPFive.Extended.Animancer;
using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Avatar.Motion;
using TPFive.Game.Avatar.Tracking;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPFive.Game.Avatar.Factory
{
    [CreateAssetMenu(fileName = "AvatarFactoryPreset", menuName = "TPFive/Avatar/Create Avatar Factory Preset")]
    public class AvatarFactoryPreset : ScriptableObject
    {
        [SerializeField]
        private string presetName;

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private bool enableToGenerateLod = false;

        /// <summary>
        /// Enable to combine meshes into one mesh, excluding the head mesh.
        /// </summary>
        [SerializeField]
        private bool enableToCombineMesh = false;

        /// <summary>
        /// Binfile is a cached file used to reduce loading time. Binfile contains:
        /// <list type="bullet">
        /// <item>Combined meshes</item>
        /// <item>Combined mesh's materials</item>
        /// <item>Combined mesh's textures</item>
        /// <item>LODs</item>
        /// </list>
        /// </summary>
        [SerializeField]
        private bool enableToLoadBinfile = false;

        [SerializeField]
        private ControllerTransition baseController;

        [SerializeField]
        private AnchorPointDefinitionCategory anchorPointCategory;

        [SerializeField]
        private CommonTransitionData sitSettings;

        [SerializeField]
        private LoopTransitionData talkSettings;

        [SerializeField]
        private AvatarMotionCategory motionCategory;

        [SerializeField]
        private AvatarTrackingSettings trackingSettings;

        public string PresetName => presetName;

        public GameObject Prefab => prefab;

        public bool EnableToGenerateLod => enableToGenerateLod;

        public bool EnableToCombineMesh => enableToCombineMesh;

        public bool EnableToLoadBinfile => enableToLoadBinfile;

        public ControllerTransition BaseController => baseController;

        public AnchorPointDefinitionCategory AnchorPointCategory => anchorPointCategory;

        public CommonTransitionData SitSettings => sitSettings;

        public LoopTransitionData TalkSettings => talkSettings;

        public AvatarMotionCategory MotionCategory => motionCategory;

        public AvatarTrackingSettings TrackingSettings => trackingSettings;
    }
}
