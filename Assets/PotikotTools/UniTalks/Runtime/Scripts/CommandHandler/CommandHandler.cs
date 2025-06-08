using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    // TODO: implement async logic
    public class CommandHandler
    {
        private const int InitialCommandsListCapacity = 20;

        private readonly List<ICommandInfo> _commands;

        public IReadOnlyList<ICommandInfo> Commands => _commands;

        public CommandHandler()
        {
            _commands = new List<ICommandInfo>(InitialCommandsListCapacity);
            SetupCommands();
        }

        private void SetupCommands()
        {
            List<string> assemblyNames = CommandHandlerPreferences.CommandAttributeUsingAssemblies;
            List<Assembly> assemblies = new(assemblyNames.Count);

            foreach (string assemblyName in assemblyNames)
                assemblies.Add(Assembly.Load(assemblyName));

            // var log = new StringBuilder()
            //     .AppendLine($"Loaded commands from assemblies({assemblies.Count}):");
            foreach (Assembly assembly in assemblies)
            {
                // log.AppendLine(assembly.FullName);
                foreach (Type type in assembly.GetTypes())
                {
                    AddMethods(type);
                    AddFields(type);
                }
            }
            
            // Debug.Log(log.ToString());
        }

        public void Execute(string commandName, params object[] parameters)
        {
            if (!TryGet(commandName, out ICommandInfo commandInfo))
            {
                Debug.LogWarning("[Console] Command not found: " + commandName);
                return;
            }
            if (commandInfo.HasParameters && (parameters == null || commandInfo.ParameterTypes.Length != parameters.Length))
            {
                Debug.LogWarning("[Console] Incorrect input parameters for command: " + commandName);
                return;
            }
            
            commandInfo.Invoke(parameters);
        }

        /// <summary>
        /// Command in format "&lt;CommandName&gt; &lt;Arg1&gt; &lt;Arg2&gt; ..."
        /// </summary>
        public void Execute(string inputCommand)
        {
            if (string.IsNullOrEmpty(inputCommand))
                return;

            string[] splittedCommand = inputCommand.Split(' ');
            if (!TryGet(splittedCommand[0], out ICommandInfo commandInfo))
            {
                Debug.LogWarning("[Console] Command not found: " + inputCommand);
                return;
            }

            if (!TryParseParameters(inputCommand, splittedCommand, commandInfo, out object[] parameters))
            {
                Debug.LogWarning("[Console] Incorrect input parameters");
                return;
            }

            commandInfo.Invoke(parameters);
        }

        public async Task ExecuteWithDelayAsync(string inputCommand, float delay)
        {
            if (string.IsNullOrEmpty(inputCommand))
                return;

            int milliseconds = (int)(delay * 1000f);
            await Task.Delay(milliseconds);
            
            Execute(inputCommand);
        }
        
        public void Register(ICommandInfo commandInfo)
        {
            if (!commandInfo.IsValid || Contains(commandInfo.Name))
                return;

            _commands.Add(commandInfo);
        }

        public void Register(string commandName, string description, Action callback)
        {
            if (callback == null || string.IsNullOrEmpty(commandName) || Contains(commandName))
                return;

            _commands.Add(new ActionCommandInfo(commandName, description, callback));
        }

        public void Register(string commandName, Action callback)
        {
            Register(commandName, null, callback);
        }

        public void Unregister(ICommandInfo commandInfo)
        {
            _commands.Remove(commandInfo);
        }

        public void Unregister(string commandName)
        {
            int index = IndexOf(commandName);

            if (index != -1)
                _commands.RemoveAt(index);
        }

        private int IndexOf(string commandName)
        {
            for (int i = 0; i < _commands.Count; i++)
                if (commandName == _commands[i].Name)
                    return i;

            return -1;
        }

        private bool TryGet(string commandName, out ICommandInfo commandInfo)
        {
            foreach (ICommandInfo command in _commands)
            {
                if (commandName == command.Name)
                {
                    commandInfo = command;
                    return true;
                }
            }

            commandInfo = null;
            return false;
        }

        private bool Contains(string commandName)
        {
            foreach (ICommandInfo command in _commands)
                if (command.Name == commandName)
                    return true;

            return false;
        }

        private bool TryParseParameters(string inputCommand, string[] splittedCommand, ICommandInfo commandInfo, out object[] parameters)
        {
            Type[] parameterTypes = commandInfo.ParameterTypes;

            if (splittedCommand.Length == 1
                && parameterTypes == null)
            {
                parameters = null;
                return true;
            }
            if (splittedCommand.Length > 1
                && parameterTypes.Length == 1
                && (parameterTypes[0] == typeof(string) || parameterTypes[0] == typeof(object)))
            {
                parameters = new object[] { inputCommand[(splittedCommand[0].Length + 1)..] };
                return true;
            }
            if (splittedCommand.Length - 1 != parameterTypes.Length)
            {
                parameters = null;
                return false;
            }
            if (splittedCommand.Length > 1)
            {
                parameters = new object[parameterTypes.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    parameters[i] = Converter.Convert(parameterTypes[i], splittedCommand[i + 1]);

                    if (parameters[i] == null)
                    {
                        parameters = null;
                        return false;
                    }
                }

                return true;
            }

            parameters = null;
            return true;
        }

        private void AddMethods(Type type)
        {
            MethodInfo[] methodInfos = type.GetMethods(CommandHandlerPreferences.ReflectionBindingFlags);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                CommandAttribute attribute = methodInfo.GetCustomAttribute<CommandAttribute>();

                if (attribute == null)
                    continue;

                string methodName = string.IsNullOrEmpty(attribute.Name) ? methodInfo.Name : attribute.Name;
                _commands.Add(new MethodCommandInfo(
                    attribute.IncludeTypeName ? $"{type.Name}.{methodName}" : methodName,
                    attribute.Description,
                    methodInfo));
            }
        }

        private void AddFields(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(CommandHandlerPreferences.ReflectionBindingFlags);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                CommandAttribute attribute = fieldInfo.GetCustomAttribute<CommandAttribute>();
                
                if (attribute == null)
                    continue;

                string fieldName = string.IsNullOrEmpty(attribute.Name) ? fieldInfo.Name : attribute.Name;
                _commands.Add(new FieldCommandInfo(
                    attribute.IncludeTypeName ? $"{type.Name}.{fieldName}" : fieldName,
                    attribute.Description,
                    fieldInfo));
            }
        }
    }
}