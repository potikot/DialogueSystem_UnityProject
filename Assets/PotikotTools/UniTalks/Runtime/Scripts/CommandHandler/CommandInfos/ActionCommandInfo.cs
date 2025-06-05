using System;

namespace PotikotTools.UniTalks
{
    public class ActionCommandInfo : ICommandInfo
    {
        private Action _callback;

        public string Name { get; private set; }
        public string Description { get; private set; }
        public object Context => null;
        public string HintText => Name;

        public Type[] ParameterTypes => null;

        public bool IsValid => _callback != null && !string.IsNullOrEmpty(Name);

        public ActionCommandInfo(string name, string description, Action callback)
        {
            Name = name.Replace(' ', '_');
            Description = description;
            _callback = callback;
        }

        public ActionCommandInfo(string name, Action callback) : this(name, null, callback) { }

        public void Invoke(object[] parameters)
        {
            _callback?.Invoke();
        }
    }
}