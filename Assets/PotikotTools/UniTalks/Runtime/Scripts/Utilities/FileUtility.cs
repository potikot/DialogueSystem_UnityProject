using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Device;

namespace PotikotTools.UniTalks
{
    /// <summary>
    /// Use absolute paths to avoid all errors. Relative paths does not work in builds, if working directory changed and if used System.Diagnostics.Process in custom pipelines
    /// </summary>
    public static class FileUtility
    {
        // TODO: paths are too easy to break. Example - replace '/' by '\'
        
        #region Sync

        public static bool Write(string absolutePath, string data, bool refreshAsset = true)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return false;
            }

            string directoryPath = Path.GetDirectoryName(absolutePath);
            Directory.CreateDirectory(directoryPath);
            
            File.WriteAllText(absolutePath, data ?? "");
            
            if (refreshAsset)
                AssetDatabase.ImportAsset(GetProjectRelativePath(absolutePath));
            
            return true;
        }
        
        public static bool WriteAllLines(string absolutePath, string[] data, bool refreshAsset = true)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return false;
            }

            string directoryPath = Path.GetDirectoryName(absolutePath);
            Directory.CreateDirectory(directoryPath);
            
            File.WriteAllLines(absolutePath, data ?? Array.Empty<string>());
            
            if (refreshAsset)
                AssetDatabase.ImportAsset(GetProjectRelativePath(absolutePath));
            
            return true;
        }

        public static string Read(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }
            
            if (!File.Exists(absolutePath))
                return null;
            
            return File.ReadAllText(absolutePath);
        }
        
        public static string[] ReadAllLines(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }
            
            if (!File.Exists(absolutePath))
                return null;
            
            return File.ReadAllLines(absolutePath);
        }
        
        #endregion

        #region Async
        
        public static async Task<bool> WriteAsync(string absolutePath, string data, bool refreshAsset = true)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return false;
            }

            string directoryPath = Path.GetDirectoryName(absolutePath);
            Directory.CreateDirectory(directoryPath);

            await File.WriteAllTextAsync(absolutePath, data ?? "");
            
            if (refreshAsset)
                AssetDatabase.ImportAsset(GetProjectRelativePath(absolutePath));
            
            return true;
        }
        
        public static async Task<string> ReadAsync(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }

            if (!File.Exists(absolutePath))
                return null;
            
            return await File.ReadAllTextAsync(absolutePath);
        }
        
        public static async Task<string[]> ReadAllLinesAsync(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }

            if (!File.Exists(absolutePath))
                return null;
            
            return await File.ReadAllLinesAsync(absolutePath);
        }
        
        #endregion
        
        public static string GetProjectRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }
            
            string normalizedDataPath = Application.dataPath.Replace("\\", "/");
            string normalizedAbs = absolutePath.Replace("\\", "/");

            if (!normalizedAbs.StartsWith(normalizedDataPath, StringComparison.OrdinalIgnoreCase))
            {
                UniTalksAPI.LogError($"'{nameof(absolutePath)}' must be inside the Assets folder ({absolutePath}).");
                return null;
            }

            int startIndex = normalizedDataPath.Length - "Assets".Length;
            return normalizedAbs[startIndex..].TrimStart('/');
        }

        public static string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                UniTalksAPI.LogError($"'{nameof(relativePath)}' cannot be null or empty.");
                return null;
            }
            
            string normalizedPath = relativePath.Replace("\\", "/").TrimStart('/');
            if (!normalizedPath.StartsWith("Assets"))
            {
                UniTalksAPI.LogError($"Path must start with 'Assets', '/Assets', or '\\Assets' ({relativePath}).");
                return null;
            }
            
            return Path.Combine(Application.dataPath, normalizedPath["Assets".Length..].TrimStart('/'));
        }

        public static bool IsDatabaseRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                UniTalksAPI.LogError($"'{nameof(path)}' cannot be null or empty.");
                return false;
            }
            
            return path.StartsWith(DialoguesComponents.Database.RelativeRootPath, StringComparison.CurrentCultureIgnoreCase);
        }
        
        // TODO: manage extensions
        public static bool MoveAssetToDatabase(Type assetType, string oldPath, string newFileName = null)
        {
            newFileName ??= Path.GetFileName(oldPath);
            string newRelativePath = DialoguesComponents.Database.GetProjectRelativeResourcePath(assetType, newFileName);
            string newFolderRelativePath = Path.GetDirectoryName(newRelativePath);
            
            if (!AssetDatabase.IsValidFolder(newFolderRelativePath))
            {
                Directory.CreateDirectory(GetAbsolutePath(newFolderRelativePath));
                AssetDatabase.ImportAsset(newFolderRelativePath);
            }

            string error = AssetDatabase.MoveAsset(oldPath, newRelativePath);
            if (!string.IsNullOrEmpty(error))
            {
                UniTalksAPI.LogError(error);
                return false;
            }
            
            return true;
        }
        
        public static bool MoveAssetToDatabase(UnityEngine.Object asset, string newFileName = null)
        {
            if (!asset)
                return false;
            
            return MoveAssetToDatabase(asset.GetType(), AssetDatabase.GetAssetPath(asset), newFileName);
        }
    }
}