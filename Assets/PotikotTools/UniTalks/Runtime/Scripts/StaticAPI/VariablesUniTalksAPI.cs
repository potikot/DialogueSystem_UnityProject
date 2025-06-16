namespace PotikotTools.UniTalks
{
    public static partial class UniTalksAPI
    {
        public static void SetVariable<T>(string key, T value) => DialoguesComponents.Variables.Set(key, value);
        public static T GetVariable<T>(string key, T defaultValue = default) => DialoguesComponents.Variables.Get(key, defaultValue);
        public static object GetRawVariable(string key) => DialoguesComponents.Variables.GetRaw(key);
        public static bool HasVariable(string key) => DialoguesComponents.Variables.Has(key);
        public static bool RemoveVariable(string key) => DialoguesComponents.Variables.Remove(key);
        public static void ClearVariables() => DialoguesComponents.Variables.Clear();
    }
}