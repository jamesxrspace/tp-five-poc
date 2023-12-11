using UnityEngine;

namespace TPFive.Game.Video
{
    public static class Helper
    {
        public static string GetFilePath(string path, VideoPathType location)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                switch (location)
                {
                    case VideoPathType.AbsolutePathOrURL:
                        result = path;
                        break;
                    case VideoPathType.RelativeToDataFolder:
                    case VideoPathType.RelativeToPersistentDataFolder:
                    case VideoPathType.RelativeToProjectFolder:
                    case VideoPathType.RelativeToStreamingAssetsFolder:
                        result = System.IO.Path.Combine(GetPath(location), path);
                        break;
                }
            }

            return result;
        }

        public static string GetPath(VideoPathType location)
        {
            string result = string.Empty;
            switch (location)
            {
                case VideoPathType.AbsolutePathOrURL:
                    break;
                case VideoPathType.RelativeToDataFolder:
                    result = Application.dataPath;
                    break;
                case VideoPathType.RelativeToPersistentDataFolder:
                    result = Application.persistentDataPath;
                    break;
                case VideoPathType.RelativeToProjectFolder:
#if !UNITY_WINRT_8_1
                    string path = "..";
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
						path += "/..";
#endif
                    result = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, path));
                    result = result.Replace('\\', '/');
#endif
                    break;
                case VideoPathType.RelativeToStreamingAssetsFolder:
                    result = Application.streamingAssetsPath;
                    break;
            }

            return result;
        }

#if UNITY_EDITOR_WIN || (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
        // Handle very long file paths by converting to DOS 8.3 format
        internal static string ConvertLongPathToShortDOS83Path(string path)
        {
            const string pathToken = @"\\?\";
            var result = pathToken + path.Replace("/", "\\");
            var length = GetShortPathName(result, null, 0);
            if (length > 0)
            {
                var sb = new System.Text.StringBuilder(length);
                if (GetShortPathName(result, sb, length) != 0)
                {
                    result = sb.ToString().Replace(pathToken, string.Empty);
                }
            }

            return result;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, EntryPoint = "GetShortPathNameW", SetLastError = true)]
        private static extern int GetShortPathName(
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pathName,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] System.Text.StringBuilder shortName,
            int cbShortName);
#endif
    }
}