using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    public static class CommandHandlerPreferences
    {
        public const BindingFlags ReflectionBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public const string FileName = "CommandHandlerRuntimePreferences";
        public static readonly string ResourceConfigsPath = "UniTalks/Configs/";

        private static CommandHandlerPreferencesSO _data;

        public static CommandHandlerPreferencesSO Data
        {
            get
            {
                if (_data == null)
                {
                    _data = Resources.Load<CommandHandlerPreferencesSO>(ResourceConfigsPath + FileName);

                    if (_data == null)
                    {
                        _data = ScriptableObject.CreateInstance<CommandHandlerPreferencesSO>();

                        #if UNITY_EDITOR

                        string relativePath = "Assets/Resources/" + ResourceConfigsPath;
                        string path = FileUtility.GetAbsolutePath(relativePath);

                        Directory.CreateDirectory(path);
                        UnityEditor.AssetDatabase.ImportAsset(relativePath);
                        UnityEditor.AssetDatabase.CreateAsset(Data, $"{relativePath}/{FileName}.asset");
                        
                        #endif
                    }
                }
                
                return _data;
            }
        }
        
        public static bool ExcludeFromSearchAssemblies
        {
            get => Data.ExcludeDefaultAssemblies;
            set => Data.ExcludeDefaultAssemblies = value;
        }
        
        public static List<string> ExcludedFromSearchAssemblyPrefixes => Data.ExcludedAssemblyPrefixes;
        public static List<string> CommandAttributeUsingAssemblies => Data.CommandAttributeUsingAssemblies;

        #if UNITY_EDITOR

        public static void Save()
        {
            if (Data == null)
                return;

            UnityEditor.EditorUtility.SetDirty(Data);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(Data);
        }

        public static void Reset()
        {
            if (Data == null)
                return;

            Data.Reset();
            Save();
        }
        
        #endif
    }
}