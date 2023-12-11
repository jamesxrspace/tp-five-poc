namespace TPFive.Game
{
    public delegate void T1MultiObjParamDelegate<T>(T t, params object[] args);

    public class Utility
    {
        public static void T1MultiObjParamAction<T>(T t, T1MultiObjParamDelegate<T> callback)
        {
            callback(t);
        }

        /// <summary>
        /// Determines the appropriate <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.html">BuildTarget</see> string based on the provided RuntimePlatform.
        /// </summary>
        /// <param name="runtimePlatform">The RuntimePlatform enum value representing the target platform.</param>
        /// <returns>A string representing the corresponding build target for the specified platform.</returns>
        /// <remarks>
        /// This method helps simplify the process of selecting the correct build target string
        /// for different platforms in Unity3D or similar game development environments. It maps
        /// the provided RuntimePlatform enum value to the appropriate build target string,
        /// allowing developers to write platform-independent code while ensuring that the
        /// correct build target is used during application builds.
        /// </remarks>
        /// <example>
        /// Usage example:
        /// <code>
        /// var platform = TPFive.Game.GameApp.Platform;
        /// string buildTarget = GetBuildTargetByRuntimePlatform(platform);
        /// // 'buildTarget' now contains the appropriate build target string.
        /// </code>
        /// </example>
        /// <seealso cref="RuntimePlatform"/>
        public static string GetBuildTargetByRuntimePlatform(RuntimePlatform runtimePlatform)
        {
            var buildTarget = runtimePlatform switch
            {
                RuntimePlatform.Android or RuntimePlatform.OculusVR => "Android",
                RuntimePlatform.IPhonePlayer => "iOS",
                RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer or RuntimePlatform.WindowsServer =>
                    "StandaloneWindows64",
                RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer or RuntimePlatform.OSXServer => "StandaloneOSX",
                RuntimePlatform.LinuxEditor or RuntimePlatform.LinuxPlayer or RuntimePlatform.LinuxServer =>
                    "StandaloneLinux64",
                RuntimePlatform.VisionOS => "VisionOS",

                // Default to windows rather them empty to avoid loading failure.
                _ => "StandaloneWindows64"
            };

            return buildTarget;
        }
    }
}
