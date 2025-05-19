using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Device;

namespace PotikotTools.DialogueSystem
{
    /// <summary>
    /// Use absolute paths to avoid all errors. Relative paths does not work in builds, if working directory changed and if used System.Diagnostics.Process in custom pipelines
    /// </summary>
    public static class FileUtility
    {
        #region Sync

        public static bool Write(string absolutePath, string data, bool refreshAsset = true)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
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
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
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
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
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
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
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
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
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
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }

            if (!File.Exists(absolutePath))
                return null;
            
            return await File.ReadAllTextAsync(absolutePath);
        }
        
        #endregion
        
        public static string GetProjectRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                DL.LogError($"'{nameof(absolutePath)}' cannot be null or empty.");
                return null;
            }
            
            string dataPath = Application.dataPath.Replace("\\", "/");
            string abs = absolutePath.Replace("\\", "/");

            if (!abs.StartsWith(dataPath, StringComparison.OrdinalIgnoreCase))
            {
                DL.LogError($"'{nameof(absolutePath)}' must be inside the Assets folder.");
                return null;
            }

            int startIndex = dataPath.Length - "Assets".Length;
            return abs[startIndex..].TrimStart('/');
        }

        public static string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                DL.LogError($"'{nameof(relativePath)}' cannot be null or empty.");
                return null;
            }
            
            string normalizedPath = relativePath.Replace("\\", "/").TrimStart('/');
            if (!normalizedPath.StartsWith("Assets"))
            {
                DL.LogError("Path must start with 'Assets', '/Assets', or '\\Assets'.");
                return null;
            }
            
            return Path.Combine(Application.dataPath, normalizedPath["Assets".Length..].TrimStart('/'));
        }
    }
}