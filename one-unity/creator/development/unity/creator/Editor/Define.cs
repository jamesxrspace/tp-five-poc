using System.IO;

namespace TPFive.Creator.Editor
{
    public static class Define
    {
        internal static readonly string CreatorEditorPath = Path.Combine(
            "Packages", "io.xrspace.tpfive.creator", "Editor");

        internal static readonly string CreatorToolPath = Path.Combine(
            "Packages", "io.xrspace.tpfive.creator", "Tools~");

        internal static readonly string CreatorExeParentPath = Path.Combine(
            Define.CreatorToolPath, "TPFive.Creator.Console");

        internal static readonly string UgcExeParentPath = Path.Combine(
            Define.CreatorToolPath, "TPFive.Ugc.Console");

        internal static readonly string BundleTexturePath = Path.Combine(
            CreatorEditorPath, "Bundle", "Textures");

        internal static readonly string BundleLevelIconPath = Path.Combine(
            BundleTexturePath, "level.png");

        internal static readonly string BundleParticleIconPath = Path.Combine(
            BundleTexturePath, "particle.png");

        internal static readonly string BundleSceneObjectIconPath = Path.Combine(
            BundleTexturePath, "scene-object.png");

        internal static readonly string ContentPath = Path.Combine("Assets", "_", "1 - Game");

        internal static readonly string LevelRelativePath =
            Path.Combine("Level", "Entry", "Scenes", "Level - Entry.unity");

        internal static readonly string LevelName = "Level - Entry.unity";
        internal static readonly string ThumbnailIconName = "Thumbnail.png";

        internal static readonly string GuidPattern =
            "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";

        internal static readonly string AddressableRuleCategoryPattern = "(?<category>[^/]+)/(?<asset>.*)";
        internal static readonly string AddressableRuleGuidPattern = "(?<asset>[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?.*)";

        // This works on all platforms supporting .Net, but only windows
        // platform has exe file extension.
        internal const string ExeName =
#if UNITY_EDITOR_WIN
            "TPFive.Creator.Console.exe";
#elif UNITY_EDITOR_OSX
            "TPFive.Creator.Console";
#elif UNITY_EDITOR_LINUX
            "TPFive.Creator.Console";
#endif

        internal const string UgcExeName =
#if UNITY_EDITOR_WIN
            "TPFive.Ugc.Console.exe";
#elif UNITY_EDITOR_OSX
            "TPFive.Ugc.Console";
#elif UNITY_EDITOR_LINUX
            "TPFive.Ugc.Console";
#endif
    }
}
