using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    public class CommandHandlerPreferencesSO : ScriptableObject
    {
        public bool ExcludeDefaultAssemblies = true;
        public List<string> ExcludedAssemblyPrefixes = new() { "Unity", "System", "mscorlib", "Mono", "netstandard", "Microsoft" };
        
        public List<string> CommandAttributeUsingAssemblies = new();
 
        public void Reset()
        {
            ExcludeDefaultAssemblies = true;
            ExcludedAssemblyPrefixes = new List<string>() { "Unity", "System", "mscorlib", "Mono", "netstandard", "Microsoft" };
            CommandAttributeUsingAssemblies = new List<string>();
        }

        public void CopyFrom(CommandHandlerPreferencesSO source)
        {
            ExcludeDefaultAssemblies = source.ExcludeDefaultAssemblies;
            ExcludedAssemblyPrefixes = new List<string>(source.ExcludedAssemblyPrefixes);
            CommandAttributeUsingAssemblies = new List<string>(source.CommandAttributeUsingAssemblies);
        }

        public CommandHandlerPreferencesSO CreateCopy()
        {
            return Instantiate(this);
        }
    }
}