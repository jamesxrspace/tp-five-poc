using System;
using System.IO;
using UnityEngine;

namespace TPFive.Game.Avatar.Factory
{
    public static class AvatarConstants
    {
        public const string BinfileRootDirName = "binfiles";
        public const string DefaultBinfileFileName = "cache.bin";

        private static string binfileRootDir;

        public static string GetBinfileRootDir()
        {
#if UNITY_EDITOR
            if (binfileRootDir == null)
            {
                // To avoid multiple processes downloading the a binfile at the same time.
                // When testing multiplayer on the same device, you may encounter this issue.
                const int shortHashLength = 8;
                var hashString = GetSHA1Hash(Application.dataPath, shortHashLength);
                binfileRootDir = Path.Combine(Application.temporaryCachePath, $"{BinfileRootDirName}-{hashString}");
            }

            return binfileRootDir;
#else
            return Path.Combine(Application.temporaryCachePath, BinfileRootDirName);
#endif
        }

#if UNITY_EDITOR
        private static string GetSHA1Hash(string inputString, int? length = null)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();

            // Convert the input string to bytes
            var data = System.Text.Encoding.UTF8.GetBytes(inputString);

            // Calculate the SHA-1 hash
            var hash = sha1.ComputeHash(data);

            // Convert the hash to a hexadecimal string
            var hashString = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            return length.HasValue ? hashString[..length.Value] : hashString;
        }
#endif
    }
}
