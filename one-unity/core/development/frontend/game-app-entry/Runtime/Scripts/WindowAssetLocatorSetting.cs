using TPFive.Game.UI;
using UnityEngine;

namespace TPFive.Game.App.Entry
{
    [CreateAssetMenu(fileName = nameof(WindowAssetLocatorSetting), menuName = "TPFive/UI/Window Asset Locator Setting")]
    public sealed class WindowAssetLocatorSetting : PlatformGroupBasedSetting<WindowAssetLocator>
    {
    }
}
