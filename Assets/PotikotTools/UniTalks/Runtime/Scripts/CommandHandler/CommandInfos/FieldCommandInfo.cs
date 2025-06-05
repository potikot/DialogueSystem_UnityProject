using System;
using System.Reflection;

namespace PotikotTools.UniTalks
{
    public class FieldCommandInfo : ICommandInfo
    {
        private string _hintText;
        private Type[] _parameterTypes;

        public string Name { get; private set; }
        public string Description { get; private set; }
        public object Context { get; private set; }
        public FieldInfo FieldInfo { get; private set; }

        public string HintText
        {
            get
            {
                if (!string.IsNullOrEmpty(_hintText))
                    return _hintText;

                _hintText = $"{Name} {ParameterTypes[0].Name}";
                return _hintText;
            }
        }

        public Type[] ParameterTypes => _parameterTypes ??= new Type[1] { FieldInfo.FieldType };

        public bool IsValid => FieldInfo != null && !string.IsNullOrEmpty(Name);

        public FieldCommandInfo(string name, string description, FieldInfo fieldInfo, object context = null)
        {
            Name = name.Replace(' ', '_');
            Description = description;
            FieldInfo = fieldInfo;
            Context = context;
        }

        public FieldCommandInfo(string name, FieldInfo fieldInfo, object context = null) : this(name, null, fieldInfo, context) { }

        public void Invoke(object[] parameters)
        {
            FieldInfo.SetValue(Context, parameters[0]);
        }
    }
}