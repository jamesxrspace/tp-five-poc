using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Unity3D;
using Splat;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Assist.Entry.Editor
{
    using TPFive.Cross.Editor;

    /// <summary>
    /// This should be called as second bootstrap if there is one already.
    /// </summary>
    [OrderedInitializeOnLoad(0x0480)]
    public sealed partial class ModuleEntry
    {
        private static void OnLoadBegin(object someParams)
        {
            Debug.Log("[TPFive.Game.Assist.Entry.Editor.ModuleEntry] - OnLoadBegin");
        }

        private static void OnLoadEnd(object someParams)
        {
            Debug.Log("[TPFive.Game.Assist.Entry.Editor.ModuleEntry] - OnLoadEnd");
        }
    }
}
