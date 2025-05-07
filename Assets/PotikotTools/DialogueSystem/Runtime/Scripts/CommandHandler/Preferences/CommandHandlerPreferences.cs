using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public static class CommandHandlerPreferences
    {
        public const BindingFlags ReflectionBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static CommandHandlerPreferencesSO _preferencesSO;

        public static CommandHandlerPreferencesSO PreferencesSO
        {
            get
            {
                if (_preferencesSO == null)
                {
                    _preferencesSO = Resources.Load<CommandHandlerPreferencesSO>("CommandHandlerRuntimePreferences");
                    
                    if (_preferencesSO == null)
                        _preferencesSO = ScriptableObject.CreateInstance<CommandHandlerPreferencesSO>();
                }
                
                return _preferencesSO;
            }
        }
        
        public static bool ExcludeFromSearchAssemblies
        {
            get => PreferencesSO.ExcludeDefaultAssemblies;
            set => PreferencesSO.ExcludeDefaultAssemblies = value;
        }
        
        public static List<string> ExcludedFromSearchAssemblyPrefixes => PreferencesSO.ExcludedAssemblyPrefixes;
        public static List<string> CommandAttributeUsingAssemblies => PreferencesSO.CommandAttributeUsingAssemblies;

        #if UNITY_EDITOR

        public static void Save()
        {
            EditorUtility.SetDirty(PreferencesSO);
            AssetDatabase.SaveAssetIfDirty(PreferencesSO);
        }

        public static void Reset()
        {
            PreferencesSO.Reset();
            Save();
        }
        
        #endif
    }
}