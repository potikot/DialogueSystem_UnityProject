using System.Collections.Generic;

namespace PotikotTools.UniTalks
{
    public class VariablesStore
    {
        private readonly Dictionary<string, object> _variables = new();

        public void Set<T>(string key, T value)
        {
            _variables[key] = value;
        }
        
        public T Get<T>(string key, T defaultValue = default)
        {
            if (_variables.TryGetValue(key, out var v))
            {
                switch (v)
                {
                    case T cv:
                        return cv;
                    case null:
                        return default;
                }
            }
            
            return defaultValue;
        }
        
        public object GetRaw(string key) => _variables.GetValueOrDefault(key);
        
        public bool Has(string key) => _variables.ContainsKey(key);
        public bool Remove(string key) => _variables.Remove(key);
        public void Clear() => _variables.Clear();
    }
}