using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PotikotTools.DialogueSystem
{
    public static class AddressableUtility
    {
        public static bool IsAddressable(this Object obj)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));
            return entry != null;
        }

        public static AssetReferenceT<T> AsAddressable<T>(this T obj) where T : Object
        {
            return new AssetReferenceT<T>(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));
        }
    }
}