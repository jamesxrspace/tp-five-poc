namespace TPFive.Game.AvatarEdit.Entry
{
    public enum AvatarEditorPage
    {
        Main = 0,
        Appearances,
        Apparels,
        Makeup,
    }

    public enum AvatarCreationPage
    {
        Main = 0,
        Appearances,
        Apparels,
        Makeup,
        Preset,
    }

    public enum AvatarCreationExit
    {
        HomeDefault,
        HomeProfile,
        AboutYourSelf,
    }

    public enum CameraMovement
    {
        Full = 0,
        Half,
    }

    public enum AvatarPose
    {
        Default,
        Focus,
        Snapshot,
    }

    public class AvatarCreationUtils
    {
        public static bool TryConvert(AvatarCreationPage entry, out AvatarEditorPage editorPage)
        {
            AvatarEditorPage? result = entry switch
            {
                AvatarCreationPage.Main => AvatarEditorPage.Main,
                AvatarCreationPage.Appearances => AvatarEditorPage.Appearances,
                AvatarCreationPage.Apparels => AvatarEditorPage.Apparels,
                AvatarCreationPage.Makeup => AvatarEditorPage.Makeup,
                _ => null,
            };

            editorPage = result.HasValue ? result.Value : AvatarEditorPage.Main;
            return result.HasValue;
        }
    }
}