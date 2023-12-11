using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Video
{
    [Serializable]
    public class VideoPath : IEquatable<VideoPath>
    {
        [SerializeField]
        private string _path = string.Empty;
        [SerializeField]
        private VideoPathType _pathType = VideoPathType.AbsolutePathOrURL;

        public VideoPath()
        {
            _path = string.Empty;
            _pathType = VideoPathType.AbsolutePathOrURL;
        }

        public VideoPath(VideoPath copy)
        {
            _path = copy.Path;
            _pathType = copy.PathType;
        }

        public VideoPath(string path, VideoPathType pathType)
        {
            _path = path;
            _pathType = pathType;
        }

        public string Path
        {
            get => _path;
            internal set => _path = value;
        }

        public VideoPathType PathType
        {
            get => _pathType;
            internal set => _pathType = value;
        }

        public static bool operator ==(VideoPath left, VideoPath right)
        {
            return EqualityComparer<VideoPath>.Default.Equals(left, right);
        }

        public static bool operator !=(VideoPath a, VideoPath b)
        {
            return !(a == b);
        }

        public string GetResolvedFilePath()
        {
            string result = Helper.GetFilePath(_path, _pathType);
#if UNITY_EDITOR_WIN || (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
            if (result.Length > 200 && !result.Contains("://"))
            {
                result = Helper.ConvertLongPathToShortDOS83Path(result);
            }
#endif
            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VideoPath);
        }

        public bool Equals(VideoPath other)
        {
            return other is not null &&
                   _path == other._path &&
                   _pathType == other._pathType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_path, _pathType);
        }

        public override string ToString()
        {
            return $"{_pathType}:{_path}";
        }
    }
}