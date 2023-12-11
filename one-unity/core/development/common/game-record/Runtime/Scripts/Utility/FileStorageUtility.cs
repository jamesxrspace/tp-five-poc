using System;
using System.IO;
using UnityEngine;

public class FileStorageUtility
{
    public static string BaseDirectory
    {
        get
        {
            var basePath = Application.persistentDataPath + "/Recordings";
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            return basePath;
        }
    }

    public static string GetUniqueFileName(string fileType, string prefix = "", string suffix = "")
    {
        return $"{prefix}{Guid.NewGuid()}{suffix}{fileType}";
    }

    public static string MoveFileDirectory(string sourceFileName, string destDirPath)
    {
        string fileName = Path.GetFileName(sourceFileName);
        string destFileName = Path.Combine(destDirPath, fileName);
        File.Move(sourceFileName, destFileName);
        return destFileName;
    }
}
