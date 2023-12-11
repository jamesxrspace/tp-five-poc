using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    /// <summary>
    /// This loads the specific scene.
    /// </summary>
    /// <remarks>
    /// The use with SetupAddressableContent is to make sure the scene is loaded after the addressable content is
    /// done setup.
    /// </remarks>
    [EditorUseLogging]
    public sealed partial class LoadSpecificScene
    {
        internal static System.Action Handle(BundleDetailData bundleDetailData)
        {
            return () =>
            {
                var path = AssetDatabase.GetAssetPath(bundleDetailData);
                var parentPath = Path.GetDirectoryName(path);
                var scenePath = Path.Combine(parentPath, Define.LevelRelativePath);
                // This open the scene, which will make the bundleDetailData null for some reason
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                var centralMappingDataPath = Path.Combine(
                    Define.CreatorEditorPath, "Data Assets", "Central Mapping Data.asset");
                var centralMappingData = AssetDatabase.LoadAssetAtPath<CentralMappingData>(centralMappingDataPath);

                var mappingLayerDataPath = Path.Combine(parentPath, "Mapping Layer Data.asset");
                var mappingLayerData = AssetDatabase.LoadAssetAtPath<MappingLayerData>(mappingLayerDataPath);

                if (centralMappingData != null && mappingLayerData != null)
                {
                    centralMappingData.currentMappingData = mappingLayerData;
                }

                SetupAddressableContent.Handle(bundleDetailData)?.Invoke();
            };
        }
    }
}
