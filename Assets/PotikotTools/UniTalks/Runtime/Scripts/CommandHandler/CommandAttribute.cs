using System;

namespace PotikotTools.UniTalks
{
    /// <summary>
    /// Works only with static methods / fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        public string Name;
        public string Description;
        public bool IncludeTypeName;

        public CommandAttribute(string name, string description, bool includeTypeName = true)
        {
            Name = name;
            Description = description;
            IncludeTypeName = includeTypeName;
        }

        public CommandAttribute(string name, bool includeTypeName = true) : this(name, string.Empty, includeTypeName) { }
        public CommandAttribute(bool includeTypeName = true) : this(string.Empty, string.Empty, includeTypeName) { }
    }
}