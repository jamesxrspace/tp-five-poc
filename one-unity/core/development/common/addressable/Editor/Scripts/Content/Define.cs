using System.IO;

namespace TPFive.Extended.Addressable.Editor
{
    public static class Define
    {
        private static readonly string PackagePath = Path.Combine(
            "Packages",
            "io.xrspace.tpfive.extended.addressable");

        internal static readonly string FetcherEditorPath = Path.Combine(
            PackagePath,
            "Editor");

        internal static readonly string FetcherToolPath = Path.Combine(
            PackagePath,
            "Tools~");

        internal static readonly string FetcherDownloadPath = Path.Combine(
            PackagePath,
            "Download~");

        internal static readonly string ExeParentPath = Path.Combine(
            Define.FetcherToolPath,
            "TPFive.Fetcher.Console");

        // This works on all platforms supporting .Net, but only windows
        // platform has exe file extension.
        internal const string ExeName =
#if UNITY_EDITOR_WIN
            "TPFive.Fetcher.Console.exe";
#elif UNITY_EDITOR_OSX
            "TPFive.Fetcher.Console";
#elif UNITY_EDITOR_LINUX
            "TPFive.Fetcher.Console";
#endif
    }
}
