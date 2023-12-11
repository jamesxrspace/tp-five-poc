using UnityEngine;

namespace TPFive.Game.Resource
{
    public enum UnsuccessfulReason
    {
        /// <summary>
        /// Represent None.
        /// </summary>
        None,

        /// <summary>
        /// Represent FormatNotSupported.
        /// </summary>
        FormatNotSupported,

        /// <summary>
        /// Represent Unknown.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// Keep TextureData simple, all the actual conversion is done through factory creation.
    /// </summary>
    public class TextureData : System.IDisposable
    {
        private Texture _texture;
        private bool _disposed = false;

        public TextureData(Texture2D texture)
            : base() => _texture = texture;

        public TextureData(byte[] bytes)
            : base()
        {
            var t2d = new Texture2D(0, 0, TextureFormat.ARGB32, true);
            t2d.LoadImage(bytes, true);
            _texture = t2d;
        }

        public static TextureData Default { get; internal set; } = CreateDefault();

        public Texture2D Texture
        {
            get
            {
                if (_texture != null)
                {
                    return (Texture2D)_texture;
                }

                // Don't accidentally return Default.Texture, this will create stackoverflow
                return (Texture2D)Default._texture;
            }
        }

        public int Width => (_texture == null) ? 0 : _texture.width;

        public int Height => (_texture == null) ? 0 : _texture.height;

        public int OriginalWidth { get; set; }

        public int OriginalHeight { get; set; }

        public bool HasOrientation { get; set; } = false;

        public int Orientation { get; set; } = 1;

        public bool Succeed { get; set; }

        public UnsuccessfulReason UnsuccessfulReason { get; set; } = UnsuccessfulReason.None;

        public static explicit operator TextureData(byte[] b) => new (b);

        public static explicit operator Texture(TextureData td) => td.Texture;

        public void SetTextureName(string name)
        {
            if (this == Default || _texture == null)
            {
                return;
            }

            _texture.name = name;
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_texture != null)
                {
                    Object.Destroy(_texture);
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return $"Width: {Width} Height: {Height} OriginalWidth: {OriginalWidth} OriginalHeight: {OriginalHeight} HasOrientation: {HasOrientation} Orientation: {Orientation}  texture format: {Texture.format}";
        }

        private static TextureData CreateDefault()
        {
            // UnityEngine.Texture2D.blackTexture
            return new TextureData(new Texture2D(0, 0, TextureFormat.ARGB32, false)
            {
                name = "default_texture_data",
            });
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlaymodeInEditor(UnityEditor.EnterPlayModeOptions options)
        {
            if (options.HasFlag(UnityEditor.EnterPlayModeOptions.DisableDomainReload))
            {
                Default = CreateDefault();
            }
        }
#endif

    }
}