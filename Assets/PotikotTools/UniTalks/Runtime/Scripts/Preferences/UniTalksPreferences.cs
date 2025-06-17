using System.IO;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    public static class UniTalksPreferences
    {
        public const string FileName = "UniTalksRuntimePreferences";
        public static readonly string ResourceConfigsPath = "UniTalks/Configs/";

        public static readonly Vector2 InitialDialogueEditorWindowSize = new(700f, 350f);
        public static readonly Vector2 InitialDialogueEditorWindowPosition = new
        (
            1920f / 2f - InitialDialogueEditorWindowSize.x / 2f,
            1080f / 2f - InitialDialogueEditorWindowSize.y / 2f
        );

        public static string[] EmptyDialogueOptions = { "Next" };
        
        public static readonly UniTalksPreferencesSO Data;

        static UniTalksPreferences()
        {
            Data = Resources.Load<UniTalksPreferencesSO>(FileName);

                if (Data == null)
                {
                    Data = ScriptableObject.CreateInstance<UniTalksPreferencesSO>();
                        
                    #if UNITY_EDITOR

                    string relativePath = "Assets/Resources/" + ResourceConfigsPath;
                    string path = FileUtility.GetAbsolutePath(relativePath);

                    Directory.CreateDirectory(path);
                    UnityEditor.AssetDatabase.ImportAsset(relativePath);
                    UnityEditor.AssetDatabase.CreateAsset(Data, $"{relativePath}/{FileName}.asset");
                        
                    #endif
                }
        }
    }
}