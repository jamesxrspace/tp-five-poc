using System.Collections.Generic;
using XR.AvatarEditing;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class AvatarFormatInfo
    {
        private const int FemaleGender = 12;
        private readonly string _name;
        private readonly AvatarFormat _format;
        private readonly AvatarType _type;

        public AvatarFormatInfo(string name, AvatarFormat format)
        {
            _name = name;
            _format = format;
            _type = ConvertToAvatarType(format.gender);
        }

        public string Name => _name;

        public AvatarFormat Format => _format;

        public AvatarType Type => _type;

        private AvatarType ConvertToAvatarType(int gender) => gender switch
        {
            FemaleGender => AvatarType.Female2,
            _ => AvatarType.Male2,
        };

        private void CopyExtraInfo(string key, AvatarFormat src, AvatarFormat target)
        {
            if (src.extra_info != null
                && src.extra_info.TryGetValue(key, out string value))
            {
                target.extra_info ??= new Dictionary<string, string>();
                target.extra_info[key] = value;
            }
        }
    }
}