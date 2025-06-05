using UnityEditor;

namespace PotikotTools.UniTalks
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
