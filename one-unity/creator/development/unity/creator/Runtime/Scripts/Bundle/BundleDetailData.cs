using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

// namespace TPFive.Creator.Editor
namespace TPFive.Creator
{
    [CreateAssetMenu(fileName = "Bundle Detail Data", menuName = "TPFive/Creator/Bundle Detail Data")]
    public class BundleDetailData : ScriptableObject
    {
        public string title;
        public string description;

        public string titleTermId;
        public string descriptionTermId;

        public BundleUsage bundleUsage;

        // Keep this for now, but don't use it. Will remove it later.
        public Texture thumbnail;

        public StorageKind storageKind;
        public BundleKind bundleKind;
        public OutcomeFormat outcomeFormat;

        [Space]

        // This will be readonly inside inspector.
        public string id = System.Guid.NewGuid().ToString();

        [Space]
        public string ugcId;

        // Will use asset reference for thumbnail from now on.
        public List<AssetReferenceTexture> thumbnails;

        // Add specific sub class for further validation later if necessary.
        public List<AssetReference> scenes;

        public List<AssetReferenceGameObject> prefabs;

        public List<AssetReferenceT<ScriptableObject>> scriptableObjects;

        public List<AssetReferenceTexture> textures;
    }

    /// <summary>
    /// This currently indicates the space kind either for fixed flow(prefix with scene)
    /// or created from designers/artists(prefix with level).
    /// </summary>
    public enum BundleUsage
    {
        SystemUse,
        DesignUse,
    }

    public enum BundleKind
    {
        Level,
        SceneObject,
        Particle,
    }

    public enum StorageKind
    {
        Remote,
        Local,
    }

    public enum OutcomeFormat
    {
        Addressable,
        Zip,
    }
}
