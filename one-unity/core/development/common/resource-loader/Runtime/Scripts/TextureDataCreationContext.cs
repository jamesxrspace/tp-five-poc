using TPFive.Game.Resource;

namespace TPFive.Extended.ResourceLoader
{
    public class TextureDataCreationContext
    {
        public byte[] Data { get; set; }

        public ImageFormat ImageFormat { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool HasOrientation { get; set; }

        public int Orientation { get; set; }
    }
}