using System.IO;
#if HAS_XSPO_EXTENDED_POOLKIT
using HellTap.PoolKit;
#endif
using Microsoft.Extensions.Logging;
using Splat;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Creator.Entry.Editor
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class SceneHandler
    {
        // TODO: Add Source Code Generator for this part
        private static ILogger _logger;

        private static ILogger Logger
        {
            get
            {
                _logger ??= Locator.Current.GetService<ILoggerFactory>()
                    .CreateLogger<SceneHandler>();

                return _logger;
            }
        }

        public static void SceneCreation(
            Scene scene,
            string sceneParentPath,
            string bundleId)
        {
            Logger.LogDebug($"SceneHandler.SceneCreation: {scene.name}");

            // TODO: Might be better to extract as json data?
            // TODO: Adding 3rd party especially the paid ones, should use scripting define to check first
            var sectionCoreGO = new GameObject("-- Core");
            SceneManager.MoveGameObjectToScene(sectionCoreGO, scene);

            // Create Lifetime Scope. This is the one defined in creator entry.
            var lifetimeScopeGO = new GameObject("Lifetime Scope");
            var lifetimeScopeComp = lifetimeScopeGO.AddComponent<TPFive.Creator.Entry.LifetimeScope>();

            SceneManager.MoveGameObjectToScene(lifetimeScopeGO, scene);

            // Create creator entry specific settings
            var settingsSO = ScriptableObject.CreateInstance<TPFive.Creator.Entry.Settings>();
            settingsSO.levelBundleId = bundleId;
            lifetimeScopeComp.Settings = settingsSO;

            var folderGUID = AssetDatabase.CreateFolder(sceneParentPath, "Data Assets");
            var folderPath = AssetDatabase.GUIDToAssetPath(folderGUID);

            var settingsSOPath = Path.Combine(folderPath, $"Settings - Entry.asset");

            // Create Manager that has the added visual scripting components
            var managerGO = new GameObject("Manager");

            managerGO.tag = "Rank3Manager";

            var variablesComp = managerGO.AddComponent<Variables>();
            managerGO.AddComponent<ScriptMachine>();
            managerGO.AddComponent<StateMachine>();

            variablesComp.declarations.Set("levelBundleId", bundleId);

            SceneManager.MoveGameObjectToScene(managerGO, scene);

            var sectionObjectPoolGO = new GameObject("-- Object Pool");
            SceneManager.MoveGameObjectToScene(sectionObjectPoolGO, scene);

            // Create PoolKit
            var poolKitSetupGO = new GameObject("PoolKit Setup");
#if HAS_XSPO_EXTENDED_POOLKIT
            poolKitSetupGO.AddComponent<PoolKitSetup>();
#endif

            SceneManager.MoveGameObjectToScene(poolKitSetupGO, scene);

            AssetDatabase.CreateAsset(settingsSO, settingsSOPath);
        }
    }
}
