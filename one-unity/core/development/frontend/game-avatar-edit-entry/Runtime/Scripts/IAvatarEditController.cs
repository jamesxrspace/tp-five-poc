using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using XR.AvatarEditing.Core;
using XR.AvatarEditing.Model;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal interface IAvatarEditController
    {
        IAvatarStyleAccessor AvatarStyleAccessor { get; }

        AvatarFormatInfo CurrentAvatarFormatInfo { get; }

        void Initialize();

        IList<AvatarFormatInfo> GetPresetAvatarFormatInfos();

        UniTask<bool> CreatePreviewAavatar(AvatarFormatInfo info);

        UniTask<bool> UploadAvatarFormat();

        UniTaskVoid ShowSelectPresetWindow();

        UniTaskVoid ShowMainEditWindow();

        UniTaskVoid ShowEditDetailWindow();

        IReadOnlyDictionary<AvatarEditorPage, AvatarStyleItem> GetStyleItems();

        void EnableLoadingPanel(bool enable);

        void GoToHomeEntry();
    }
}
