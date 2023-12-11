#pragma warning disable SA1300 // Element should begin with upper-case letter
using System;
using System.ComponentModel;

namespace TPFive.Game
{
    /// <summary>
    /// Return by <see cref="GameApp.Platform"/>.
    /// </summary>
    public enum Platform
    {
        Undefined,
        iOS,
        Android,
        Windows,
        MacOS,
        OculusVR,
        VisionOS,
    }

    /// <summary>
    /// Return by <see cref="GameApp.PlatformGroup"/>.
    /// </summary>
    public enum PlatformGroup
    {
        Undefined,
        MobilePhone,
        VRHeadset,
        Standalone,
        Console,
    }

    /// <summary>
    /// Return by <see cref="GameApp.PlatformFlags"/>.
    /// </summary>
    [Flags]
    public enum PlatformFlags
    {
        None = 0,
        iOS = 1 << Platform.iOS,
        Android = 1 << Platform.Android,
        Windows = 1 << Platform.Windows,
        MacOS = 1 << Platform.MacOS,
        OculusVR = 1 << Platform.OculusVR,
        VisionOS = 1 << Platform.VisionOS,
        Everything = iOS | Android | Windows | MacOS | OculusVR | VisionOS,
    }

    /// <summary>
    /// The platform application is running. Returned by <see cref="GameApp.Platform"/>.
    /// </summary>
    public enum RuntimePlatform
    {
        /// <summary>
        /// In the Unity editor on macOS.
        /// </summary>
        OSXEditor = 0,

        /// <summary>
        /// In the player on macOS.
        /// </summary>
        OSXPlayer = 1,

        /// <summary>
        /// In the player on Windows.
        /// </summary>
        WindowsPlayer = 2,

        /// <summary>
        /// In the web player on macOS.
        /// </summary>
        [Obsolete("WebPlayer export is no longer supported in Unity 5.4+.", true)]
        OSXWebPlayer = 3,

        /// <summary>
        /// In the Dashboard widget on macOS.
        /// </summary>
        [Obsolete("Dashboard widget on Mac OS X export is no longer supported in Unity 5.4+.", true)]
        OSXDashboardPlayer = 4,

        /// <summary>
        /// In the web player on Windows.
        /// </summary>
        [Obsolete("WebPlayer export is no longer supported in Unity 5.4+.", true)]
        WindowsWebPlayer = 5,

        /// <summary>
        /// In the Unity editor on Windows.
        /// </summary>
        WindowsEditor = 7,

        /// <summary>
        /// In the player on the iPhone.
        /// </summary>
        IPhonePlayer = 8,

        /// <summary>
        /// In the player on the XBox360.
        /// </summary>
        [Obsolete("Xbox360 export is no longer supported in Unity 5.5+.")]
        XBOX360 = 10,

        /// <summary>
        /// In the player on the PS3.
        /// </summary>
        [Obsolete("PS3 export is no longer supported in Unity >=5.5.")]
        PS3 = 9,

        /// <summary>
        /// In the player on Android devices.
        /// </summary>
        Android = 11,

        /// <summary>
        /// In the player on NaCl.
        /// </summary>
        [Obsolete("NaCl export is no longer supported in Unity 5.0+.")]
        NaCl = 12,

        /// <summary>
        /// In the player on Flush player.
        /// </summary>
        [Obsolete("FlashPlayer export is no longer supported in Unity 5.0+.")]
        FlashPlayer = 0xF,

        /// <summary>
        /// In the player on Linux.
        /// </summary>
        LinuxPlayer = 13,

        /// <summary>
        /// In the Unity editor on Linux.
        /// </summary>
        LinuxEditor = 0x10,

        /// <summary>
        /// In the player on WebGL
        /// </summary>
        WebGLPlayer = 17,

        [Obsolete("Use WSAPlayerX86 instead")]
        MetroPlayerX86 = 18,

        /// <summary>
        /// In the player on Windows Store Apps when CPU architecture is X86.
        /// </summary>
        WSAPlayerX86 = 18,
        [Obsolete("Use WSAPlayerX64 instead")]
        MetroPlayerX64 = 19,

        /// <summary>
        /// In the player on Windows Store Apps when CPU architecture is X64.
        /// </summary>
        WSAPlayerX64 = 19,

        [Obsolete("Use WSAPlayerARM instead")]
        MetroPlayerARM = 20,

        /// <summary>
        /// In the player on Windows Store Apps when CPU architecture is ARM.
        /// </summary>
        WSAPlayerARM = 20,
        [Obsolete("Windows Phone 8 was removed in 5.3")]
        WP8Player = 21,
        [Obsolete("BB10Player export is no longer supported in Unity 5.4+.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        BB10Player = 22,
        [Obsolete("BlackBerryPlayer export is no longer supported in Unity 5.4+.")]
        BlackBerryPlayer = 22,
        [Obsolete("TizenPlayer export is no longer supported in Unity 2017.3+.")]
        TizenPlayer = 23,
        [Obsolete("PSP2 is no longer supported as of Unity 2018.3")]
        PSP2 = 24,

        /// <summary>
        /// In the player on the Playstation 4.
        /// </summary>
        PS4 = 25,
        [Obsolete("PSM export is no longer supported in Unity >= 5.3")]
        PSM = 26,

        /// <summary>
        /// In the player on Xbox One.
        /// </summary>
        XboxOne = 27,
        [Obsolete("SamsungTVPlayer export is no longer supported in Unity 2017.3+.")]
        SamsungTVPlayer = 28,
        [Obsolete("Wii U is no longer supported in Unity 2018.1+.")]
        WiiU = 30,

        /// <summary>
        /// In the player on the Apple's tvOS.
        /// </summary>
        tvOS = 0x1F,

        /// <summary>
        /// In the player on Nintendo Switch.
        /// </summary>
        Switch = 0x20,
        [Obsolete("Lumin is no longer supported in Unity 2022.2")]
        Lumin = 33,

        /// <summary>
        /// In the player on Stadia.
        /// </summary>
        Stadia = 34,

        /// <summary>
        /// Obsolete: Use RuntimePlatform.LinuxPlayer instead.
        /// </summary>
        [Obsolete("Use LinuxPlayer instead")]
        CloudRendering = 35,

        [Obsolete("GameCoreScarlett is deprecated, please use GameCoreXboxSeries (UnityUpgradable) -> GameCoreXboxSeries", false)]
        GameCoreScarlett = -1,
        GameCoreXboxSeries = 36,
        GameCoreXboxOne = 37,

        /// <summary>
        /// In the player on the Playstation 5.
        /// </summary>
        PS5 = 38,
        EmbeddedLinuxArm64 = 39,
        EmbeddedLinuxArm32 = 40,
        EmbeddedLinuxX64 = 41,
        EmbeddedLinuxX86 = 42,

        /// <summary>
        /// In the server on Linux.
        /// </summary>
        LinuxServer = 43,

        /// <summary>
        /// In the server on Windows.
        /// </summary>
        WindowsServer = 44,

        /// <summary>
        /// In the server on macOS.
        /// </summary>
        OSXServer = 45,
        QNXArm32 = 46,
        QNXArm64 = 47,
        QNXX64 = 48,
        QNXX86 = 49,

        /// <summary>
        /// In the player on Oculus VR headsets.
        /// </summary>
        OculusVR = 100,

        /// <summary>
        /// In the player on Apple Vision Pro.
        /// </summary>
        VisionOS = 101,
    }
}
#pragma warning restore SA1300 // Element should begin with upper-case letter
