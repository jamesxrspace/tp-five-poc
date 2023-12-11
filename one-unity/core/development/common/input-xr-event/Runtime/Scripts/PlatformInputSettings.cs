using TPFive.Game;
using UnityEngine;

namespace TPFive.Extended.InputXREvent
{
    [CreateAssetMenu(fileName = nameof(PlatformInputSettings), menuName = "TPFive/Extended/InputXREvent/Platform Input Settings")]
    public class PlatformInputSettings : PlatformGroupBasedSetting<InputSettings>
    {
        public float WaitingBufferTime => GetAsset(GameApp.PlatformGroup).WaitingBufferTime;

        public float ClickThreshold => GetAsset(GameApp.PlatformGroup).ClickThreshold;

        public float LongPressThreshold => GetAsset(GameApp.PlatformGroup).LongPressThreshold;
    }
}