using UnityEngine;

namespace TPFive.Game.UI
{
    public enum WindowManagerType
    {
        /// <summary>
        /// LoxodonFramework WindowManager as Default.
        /// </summary>
        Default,
    }

    [CreateAssetMenu(fileName = "UI Root Type", menuName = "TPFive/UI/UI Root Type")]
    public class UIRootType : ScriptableObject
    {
        public string RootName => name;
    }
}
