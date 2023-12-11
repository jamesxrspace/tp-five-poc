using System;

namespace TPFive.Room
{
    public enum PhotonRegion
    {
        /// <summary>
        /// No Specified Region
        /// </summary>
        NONE,

        /// <summary>
        /// Asia (Singapore)
        /// </summary>
        ASIA,

        /// <summary>
        /// Chinese Mainland (Shanghai)
        /// </summary>
        CN,

        /// <summary>
        /// Japan (Tokyo)
        /// </summary>
        JP,

        /// <summary>
        /// Europe (Amsterdam)
        /// </summary>
        EU,

        /// <summary>
        /// South America (Sao Paulo)
        /// </summary>
        SA,

        /// <summary>
        /// South Korea (Seoul)
        /// </summary>
        KR,

        /// <summary>
        /// USA, East (Washington D.C.)
        /// </summary>
        US,

        /// <summary>
        /// USA, West (San José)
        /// </summary>
        USW,
    }

    public static class PhotonRegionExtension
    {
        public static bool IsValid(this PhotonRegion region)
        {
            return Enum.IsDefined(typeof(PhotonRegion), region);
        }

        public static string ToName(this PhotonRegion region)
        {
            // Verify the given region
            if (!region.IsValid())
            {
                throw new Exception($"Invalid value({region}) of type {nameof(PhotonRegion)}");
            }

            // return string of the given region
            if (region != PhotonRegion.NONE)
            {
                return region.ToString().ToLowerInvariant();
            }

            return string.Empty;
        }

        public static PhotonRegion FromName(this string regionStr)
        {
            if (regionStr == null)
            {
                throw new Exception($"Invalid value({regionStr}) of type {nameof(PhotonRegion)}");
            }

            regionStr = regionStr.Trim().ToLowerInvariant();

            if (regionStr != string.Empty)
            {
                return (PhotonRegion)Enum.Parse(typeof(PhotonRegion), regionStr, true);
            }

            return PhotonRegion.NONE;
        }
    }
}