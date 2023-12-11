using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Creator.Cross.Editor
{
    /// <summary>
    /// The name might be confuse with creator usage, but this is meant to be using some functionality
    /// provided by entry package.
    /// </summary>
    public class Bridge
    {
        // Assign at entry package
        public delegate void SceneCreationDelegate(Scene scene, string sceneParentPath, string bundleId);
        public static SceneCreationDelegate SceneCreation;
    }
}
