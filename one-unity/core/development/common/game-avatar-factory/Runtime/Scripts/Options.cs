using System;

namespace TPFive.Game.Avatar.Factory
{
    /// <summary>
    /// Provide flags to select the features you need.
    /// </summary>
    public class Options : OptionBase
    {
        public static readonly Options All = new () { Features = FeatureFlags.All };

        private string binfileUrl = null;
        private bool skipBinfileLOD0 = false;

        [Flags]
        public enum FeatureFlags
        {
            /// <summary>
            /// Load model only.
            /// </summary>
            None = 0,

            /// <summary>
            /// Let avatar can play motions, such as <see cref="IAnchorPointProvider"/> and <see cref="IAvatarMotionManager"/> on the avatar.
            /// </summary>
            Motion = 1 << 1,

            /// <summary>
            /// All options.
            /// </summary>
            All = Motion,
        }

        public FeatureFlags Features { get; set; } = FeatureFlags.None;

        public bool? EnableToGenerateLod { get; set; }

        public bool? EnableToCombineMesh { get; set; }

        public bool? EnableToLoadBinfile => !string.IsNullOrEmpty(binfileUrl);

        public string BinfileUrl => binfileUrl;

        public bool SkipBinfileLOD0 => skipBinfileLOD0;

        public void EnableToLoadBindfile(string url, bool skipLOD0 = true)
        {
            binfileUrl = url;
            skipBinfileLOD0 = skipLOD0;
        }

        public void DisableToLoadBindFile()
        {
            binfileUrl = null;
            skipBinfileLOD0 = false;
        }
    }
}