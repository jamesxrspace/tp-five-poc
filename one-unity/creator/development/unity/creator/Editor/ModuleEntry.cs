using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Serilog.Sinks.Unity3D;
using Splat;
using UnityEditor;
using UnityEngine;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Creator.Editor
{
    using TPFive.Cross.Editor;

    // using CrossBridge = TPFive.Cross.Editor.Bridge;

    // TODO: This experimental usage should be removed later if not necessary.
    // public class ActivationForViewFetcher : IActivationForViewFetcher
    // {
    //     public int GetAffinityForView(Type view)
    //     {
    //         return 1;
    //     }
    //
    //     public IObservable<bool> GetActivationForView(IActivatableView view)
    //     {
    //         return Observable.Start(() =>
    //         {
    //             return true;
    //         });
    //     }
    // }

    /// <summary>
    /// This should be called according to order
    /// </summary>
    [OrderedInitializeOnLoad(0x4000)]
    public sealed partial class ModuleEntry
    {
        static void OnLoadBegin(object someParams)
        {
            Debug.Log("[TPFive.Creator.Editor.ModuleEntry] - OnLoadBegin");

            var exsitedLoggerFactory = Locator.Current.GetService<ILoggerFactory>();
            if (exsitedLoggerFactory == null)
            {
                Debug.Log("[TPFive.Creator.Editor.ModuleEntry] - OnLoadBegin - Create LoggerFactory");

                //
                var loggerConfig = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Unity3D(outputTemplate: "[{Level:u3}][{SourceContext}] {Message:j}{NewLine}{Exception}\n")
                    .WriteTo.Seq("http://localhost:5341");

                loggerConfig.MinimumLevel.Debug();

                var logger = loggerConfig.CreateLogger();
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddSerilog(logger);

                Locator.CurrentMutable
                    .Register<ILoggerFactory>(() => loggerFactory);
            }
        }

        static partial void RegisterLoggerFactory();

        static void OnLoadEnd(object someParams)
        {
            Debug.Log("[TPFive.Creator.Editor.ModuleEntry] - OnLoadEnd");

            UnityEditor.Editor.finishedDefaultHeaderGUI -= HandleGameObjectHeader;
            UnityEditor.Editor.finishedDefaultHeaderGUI += HandleGameObjectHeader;
        }


        private const int StartingIndex = 20;
        private const int EndingIndex = 29;
        private static CentralMappingData centralMappingData = default;

        private static void HandleGameObjectHeader(UnityEditor.Editor editor)
        {
            if (editor.targets.Length > 1)
            {
                return;
            }

            var gameObject = editor.target as GameObject;
            if (gameObject == null) return;

            // TODO: Remove the comment later if checking only for ComponentBase is not sufficient.
            // var componentBases = gameObject.GetComponents<ComponentBase>();
            // if (componentBases.Length == 0) return;

            if (ModuleEntry.centralMappingData == null)
            {
                var path = Path.Combine(
                    Define.CreatorEditorPath,
                    "Data Assets",
                    "Central Mapping Data.asset");
                centralMappingData = AssetDatabase.LoadAssetAtPath<CentralMappingData>(path);
            }

            // Debug.Log($"centralMappingData: {centralMappingData}");
            // Debug.Log($"centralMappingData.currentMappingData: {centralMappingData.currentMappingData}");

            var startingIndex = StartingIndex;
            var endingIndex = EndingIndex;
            var selected = 0;
            var options = new List<string>();
            // Define default name here, might be modified later.
            if (centralMappingData == null || centralMappingData.currentMappingData == null)
            {
                options = new List<string>
                {
                    "Out Scope",
                    "Layer 1",
                    "Layer 2",
                    "Layer 3",
                    "Layer 4",
                    "Layer 5",
                    "Layer 6",
                    "Layer 7",
                    "Layer 8"
                };
            }
            else
            {
                options = centralMappingData.currentMappingData.mappingList.Select(x => x.name).ToList();
                options.Insert(0, "Out Scope");
                startingIndex = centralMappingData.currentMappingData.mappingList.First().key - 1;
                endingIndex = centralMappingData.currentMappingData.mappingList.Last().key + 1;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Creator Scope");
            // How to get 21, 28? const maybe?
            if (gameObject.layer <= startingIndex || gameObject.layer >= endingIndex)
            {
                selected = 0;
            }
            else
            {
                selected = (gameObject.layer - (startingIndex + 1)) + 1;
            }
            // gameObject.layer = EditorGUILayout.LayerField("", gameObject.layer);
            selected = EditorGUILayout.Popup("Layer", selected, options.ToArray());
            var changedLayer = (selected + (startingIndex + 1) - 1);
            if (changedLayer > startingIndex && changedLayer < endingIndex)
            {
                gameObject.layer = changedLayer;
            }
            // EditorGUILayout.IntField(gameObject.layer);
            EditorGUILayout.EndVertical();
        }
    }
}
