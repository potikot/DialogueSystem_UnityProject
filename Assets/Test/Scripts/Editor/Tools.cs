using System.Diagnostics;
using PotikotTools.UniTalks.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Tools
{
    [MenuItem("Tools/Check Open Handles")]
    public static void CheckOpenHandles()
    {
        var process = Process.GetCurrentProcess();
        Debug.Log($"Current Unity.exe handle count: {process.HandleCount}");
    }

    [MenuItem("Tools/Destroy All Dialogue Editors")]
    public static void DestroyAllDialogueEditors()
    {
        foreach (var window in Resources.FindObjectsOfTypeAll<DialogueEditorWindow>())
        {
            Object.DestroyImmediate(window);
        }
    }
    
    [MenuItem("Tools/Check Inspector Window")]
    public static void CheckInspectorWindow()
    {
        InspectorUtility.CreateInspectorWindow(new GameObject().AddComponent<DialogueStarter>());
    }
}