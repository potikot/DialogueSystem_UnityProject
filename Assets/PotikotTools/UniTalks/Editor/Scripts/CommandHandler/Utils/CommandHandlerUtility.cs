using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PotikotTools.UniTalks.Editor
{
    public static class CommandHandlerUtility
    {
        public static List<Assembly> GetCommandUserAssemblies()
        {
            List<Assembly> result = new();
            foreach (Assembly assembly in GetDeveloperAssemblies())
            {
                if (assembly.GetTypes().Any(HasCommandAttribute))
                {
                    result.Add(assembly);
                }
            }

            return result;
        }
        
        public static bool HasCommandAttribute(Type type)
        {
            foreach (FieldInfo memberInfo in type.GetFields(CommandHandlerPreferences.ReflectionBindingFlags))
                if (memberInfo.GetCustomAttribute<CommandAttribute>() != null)
                    return true;
            foreach (MethodInfo memberInfo in type.GetMethods(CommandHandlerPreferences.ReflectionBindingFlags))
                if (memberInfo.GetCustomAttribute<CommandAttribute>() != null)
                    return true;

            return false;
        }
        
        public static IEnumerable<Assembly> GetDeveloperAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.Where(IsAssemblyIncluded);
        }

        public static bool IsAssemblyIncluded(Assembly assembly)
        {
            if (!CommandHandlerPreferences.ExcludeFromSearchAssemblies)
                return true;
            
            return !CommandHandlerPreferences.ExcludedFromSearchAssemblyPrefixes.Any(prefix => assembly.GetName().Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}