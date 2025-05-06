using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

public static class FileUtility
{
    [MenuItem("Tools/Check Open Handles")]
    public static void CheckOpenHandles()
    {
        var process = Process.GetCurrentProcess();
        Debug.Log($"Current Unity.exe handle count: {process.HandleCount}");
    }

    [MenuItem("Tools/AssetDatabase ForceUpdate Refresh")]
    public static void ForceUpdate()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
}