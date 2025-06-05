using System;
using System.Reflection;
using System.Text;

namespace PotikotTools.UniTalks
{
    public class MethodCommandInfo : ICommandInfo
    {
        private string _hintText;

        private Type[] _parameterTypes;

        public string Name { get; private set; }
        public string Description { get; private set; }
        public object Context { get; private set; }
        public MethodInfo MethodInfo { get; private set; }

        public string HintText
        {
            get
            {
                if (!string.IsNullOrEmpty(_hintText))
                    return _hintText;

                StringBuilder sb = new(Name);
                Type[] parameterTypes = ParameterTypes;

                if (parameterTypes != null)
                {
                    foreach (Type type in parameterTypes)
                    {
                        sb.Append(' ');
                        sb.Append(type.Name);
                    }
                }

                _hintText = sb.ToString();
                return _hintText;
            }
        }

        public Type[] ParameterTypes
        {
            get
            {
                if (_parameterTypes == null)
                {
                    ParameterInfo[] parameterInfos = MethodInfo.GetParameters();

                    if (parameterInfos.Length > 0)
                    {
                        _parameterTypes = new Type[parameterInfos.Length];

                        for (int i = 0; i < parameterInfos.Length; i++)
                            _parameterTypes[i] = parameterInfos[i].ParameterType;
                    }
                    else
                        _parameterTypes = Type.EmptyTypes;
                }

                return _parameterTypes;
            }
        }

        public bool IsValid => MethodInfo != null && !string.IsNullOrEmpty(Name);

        public MethodCommandInfo(string name, string description, MethodInfo methodInfo, object context = null)
        {
            Name = name.Replace(' ', '_');
            Description = description;
            MethodInfo = methodInfo;
            Context = context;
        }

        public MethodCommandInfo(string name, MethodInfo methodInfo, object context = null) : this(name, null, methodInfo, context) { }

        public void Invoke(object[] parameters)
        {
            MethodInfo.Invoke(Context, parameters);
        }
    }
}