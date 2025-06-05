using System;

namespace PotikotTools.UniTalks
{
    public interface ICommandInfo
    {
        string Name { get; }
        string Description { get; }
        object Context { get; }
        string HintText { get; }

        Type[] ParameterTypes { get; }

        bool IsValid { get; }

        virtual bool HasParameters => ParameterTypes is { Length: > 0 };
        
        void Invoke(object[] parameters);
    }
}