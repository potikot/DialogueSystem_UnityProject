using System;

namespace PotikotTools.DialogueSystem
{
    public interface ICommandInfo
    {
        string Name { get; }
        string Description { get; }
        object Context { get; }
        string HintText { get; }

        public Type[] ParameterTypes { get; }

        public bool IsValid { get; }

        void Invoke(object[] parameters);
    }
}