using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public static class DomainReloadUtility
    {
        [MenuItem("Tools/Reload Domain")]
        public static void ReloadDomain()
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
        }
    }
}
