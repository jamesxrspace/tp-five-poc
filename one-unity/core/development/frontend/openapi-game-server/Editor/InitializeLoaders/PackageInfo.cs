using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TPFive.OpenApi.GameServer
{
    [InitializeOnLoad]
    internal class PackageInfo
    {
        public const string PackageName = "io.xrspace.tpfive.openapi.game-server";

        private static ListRequest listRequest;

        static PackageInfo()
        {
            listRequest = Client.List(true);
            EditorApplication.update += Initialize;
        }

        public static bool IsReady { get; private set; }

        public static bool IsEmbedded { get; private set; }

        public static string RootDir { get; private set; }

        private static void Initialize()
        {
            if (listRequest == null || !listRequest.IsCompleted)
            {
                return;
            }

            // Unregister update event
            EditorApplication.update -= Initialize;

            // Check whether is embedded package
            if (listRequest.Status == StatusCode.Success)
            {
                var myPackageInfo = listRequest.Result.FirstOrDefault(pck => pck.name.Equals(PackageName));
                if (myPackageInfo == null)
                {
                    listRequest = null;
                    Debug.LogError($"Can't find '{PackageName}' package.");
                    return;
                }

                RootDir = myPackageInfo.resolvedPath;
                IsEmbedded = myPackageInfo.source == PackageSource.Embedded;
            }
            else if (listRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError(listRequest.Error.message);
            }

            // Clear request
            listRequest = null;

            // Raise flag
            IsReady = true;
        }
    }
}