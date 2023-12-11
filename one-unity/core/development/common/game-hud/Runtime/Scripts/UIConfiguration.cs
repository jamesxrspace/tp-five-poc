using System;
using UnityEngine;

namespace TPFive.Game.Hud
{
    public interface IUIConfiguration
    {
        string GetRootDirName();
    }

    public abstract class UIConfigurationBase : ScriptableObject, IUIConfiguration
    {
        public abstract string GetRootDirName();
    }

    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "TPFive/UI/UIConfiguration")]
    public class UIConfiguration : UIConfigurationBase
    {
        [SerializeField]
        private string rootDirName;

        public override string GetRootDirName() => rootDirName;
    }
}
