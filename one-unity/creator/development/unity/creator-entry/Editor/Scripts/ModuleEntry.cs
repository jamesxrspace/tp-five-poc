using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Unity3D;
using Splat;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.Entry.Editor
{
    using TPFive.Cross.Editor;

    using CreatorCrossEditorBridge = TPFive.Creator.Cross.Editor.Bridge;

    /// <summary>
    /// This should be called as second bootstrap if there is one already.
    /// </summary>
    [OrderedInitializeOnLoad(0x4200)]
    public sealed partial class ModuleEntry
    {
        private static void OnLoadBegin(object someParams)
        {
            Debug.Log("[TPFive.Creator.Entry.Editor.ModuleEntry] - OnLoadBegin");

            CreatorCrossEditorBridge.SceneCreation += SceneHandler.SceneCreation;
        }

        private static void OnLoadEnd(object someParams)
        {
            Debug.Log("[TPFive.Creator.Entry.Editor.ModuleEntry] - OnLoadEnd");
        }
    }
}
