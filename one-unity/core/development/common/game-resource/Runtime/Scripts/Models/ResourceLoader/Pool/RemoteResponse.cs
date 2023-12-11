namespace TPFive.Game.Resource
{
    public class RemoteResponse : System.IDisposable
    {
        public bool Valid { get; set; }

        public byte[] Data { get; set; }

        public string InfoMessage { get; set; }

        public ImageFormat ImageFormat { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool HasOrientation { get; set; }

        /// <summary>
        /// Gets or sets the Exif Orientation Tag:
        /// Reference: http://sylvana.net/jpegcrop/exif_orientation.html.
        /// </summary>
        /// <value>
        /// Normal = 1,
        /// FlipHorizontal = 2,
        /// Rotate180 = 3,
        /// FlipVertical = 4,
        /// Transpose = 5,
        /// Rotate90 = 6,
        /// Transverse = 7,
        /// Rotate270 = 8.
        /// </value>
        public int Orientation { get; set; }

        public string ErrorMessage { get; set; }

        public void Dispose()
        {
            Data = null;
        }
    }
}