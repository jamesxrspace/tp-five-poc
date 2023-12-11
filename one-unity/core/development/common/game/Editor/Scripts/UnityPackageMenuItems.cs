using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// Menu items for Unity packages.
    /// </summary>
    public class UnityPackageMenuItems
    {
        [MenuItem("TPFive/Unity Packages/Rearrange package list")]
        private static void RearrangePackageList()
        {
            string identifier = "file:" + GetEmptyPackageRelativePath();
            var addRequest = Client.Add(identifier);
            while (!addRequest.IsCompleted)
            {
            }

            switch (addRequest.Status)
            {
                case StatusCode.Success:
                    Debug.Log($"Add package from \"{identifier}\" successfully.");
                    break;
                case StatusCode.Failure:
                    Debug.Log($"Add package from \"{identifier}\" unsuccessfully. " + addRequest.Error.ToString());
                    break;
                default:
                    break;
            }

            const string packageName = "io.xrspace.rearrange-package-list";
            var removeRequest = Client.Remove(packageName);
            while (!removeRequest.IsCompleted)
            {
            }

            switch (removeRequest.Status)
            {
                case StatusCode.Success:
                    Debug.Log($"Remove package \"{packageName}\" successfully.");
                    break;
                case StatusCode.Failure:
                    Debug.Log($"Remove package \"{packageName}\" unsuccessfully. " + removeRequest.Error.ToString());
                    break;
                default:
                    break;
            }
        }

        private static string GetEmptyPackageRelativePath()
        {
            string packageRootPath = GetPackageRootPath();
            string emptyPackagePath = Path.GetFullPath(Path.Combine(packageRootPath, "Editor", "rearrange-package-list.tgz"));
            string embedPackageRootPath = Application.dataPath.Replace("Assets", "Packages");

            // Require trailing backslash for path
            if (!embedPackageRootPath.EndsWith("\\"))
            {
                embedPackageRootPath += "\\";
            }

            Uri emptyPackageUri = new Uri(emptyPackagePath);
            Uri embedPackageRootUri = new Uri(embedPackageRootPath);
            Uri relativeUri = embedPackageRootUri.MakeRelativeUri(emptyPackageUri);

            return relativeUri.ToString();
        }

        private static string GetPackageRootPath([CallerFilePath] string path = null)
        {
            return Path.Combine(path, "..", "..", "..");
        }
    }
}