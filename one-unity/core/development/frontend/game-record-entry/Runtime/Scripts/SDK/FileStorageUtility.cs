using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public class FileStorageUtility
    {
        public static string GetPersitentDataPath(string folderName = "", bool createIfNotExist = false)
        {
            var basePath = Path.Join(Application.persistentDataPath, folderName);
            if (createIfNotExist && !Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            return basePath;
        }

        public static UniTask<bool> SaveVideoToGallery(string originalFilePath, string fileName, string gallerySubFolderName = "")
        {
            if (!File.Exists(originalFilePath))
            {
                Debug.LogError("FileStorageUtility : SaveVideoToGallery() : File not found");
                return UniTask.FromResult(false);
            }

            var tcs = new UniTaskCompletionSource<bool>();
            gallerySubFolderName = string.IsNullOrEmpty(gallerySubFolderName) ? Application.productName : gallerySubFolderName;

            NativeGallery.Permission permission =
                NativeGallery.SaveVideoToGallery(
                    originalFilePath,
                    gallerySubFolderName,
                    fileName,
                    (success, path) =>
                    {
                        Debug.Log($"Media save result, State: {success} ,Path : {path}");
                        tcs.TrySetResult(success);
                    });

            Debug.Log("FileStorageUtility : Permission result: " + permission);
            return tcs.Task;
        }

        public static void SaveVideoToGallery(byte[] mediaBytes, string fileName, string folderName = "")
        {
            folderName = string.IsNullOrEmpty(folderName) ? Application.productName : folderName;

            NativeGallery.Permission permission =
                NativeGallery.SaveVideoToGallery(
                    mediaBytes,
                    folderName,
                    fileName,
                    (success, path) => Debug.Log("Media save result: " + success + " " + path));

            Debug.Log("FileStorageUtility : Permission result: " + permission);
        }
    }
}
