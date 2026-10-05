using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template.Features.Cheat
{
    public class CheatService : ICheatService
    {
        private readonly Dictionary<BaseCheatHandler, List<CheatCommandSettings>> _handlers = new();
        public event Action UpdatedCheatCollection;

        public void Init()
        {
        }

        public IReadOnlyList<CheatCommandDescriptor> GetCheatDescriptors()
        {
            return _handlers
                .SelectMany(pair => pair.Value)
                .Select(CreateCommandDescriptor)
                .ToList();
        }

        public void Execute(string input)
        {
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                throw new ArgumentException("Input string is empty");
            }

            string commandName = parts[0];
            if (!TryGetCheatCommand(commandName, out CheatCommandSettings command))
            {
                throw new ArgumentException($"Unknown cheat: {commandName}");
            }

            object[] args = ParseArguments(parts.Skip(1).ToArray(), command.Info);
            command.Info.Invoke(command.Handler, args);
        }

        public void Register(BaseCheatHandler handler)
        {
            _handlers[handler] = GetCheatMethods(handler);
            UpdatedCheatCollection?.Invoke();
        }

        public void Unregister(BaseCheatHandler handler)
        {
            _handlers.Remove(handler);
            UpdatedCheatCollection?.Invoke();
        }

        private object[] ParseArguments(string[] inputArgs, MethodInfo method)
        {
            var parameters = method.GetParameters();
            object[] result = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i >= inputArgs.Length)
                {
                    result[i] = parameters[i].HasDefaultValue
                        ? parameters[i].DefaultValue
                        : GetDefault(parameters[i].ParameterType);
                    continue;
                }

                result[i] = Convert.ChangeType(inputArgs[i], parameters[i].ParameterType);
            }

            return result;
        }

        private CheatCommandDescriptor CreateCommandDescriptor(CheatCommandSettings settings)
        {
            return new CheatCommandDescriptor
            {
                Name = settings.Name,
                GroupName = settings.GroupName,
                Arguments = CreateCommandArgumentDescriptors(settings.Info)
            };
        }
        
        private List<CheatArgumentDescriptor> CreateCommandArgumentDescriptors(MethodInfo info)
        {
            return info.GetParameters().Select(CreateArgumentDescriptor).ToList();
        }

        private CheatArgumentDescriptor CreateArgumentDescriptor(ParameterInfo info)
        {
            return new CheatArgumentDescriptor
            {
                Name = info.GetCustomAttribute<CheatArgumentAttribute>()?.Name ?? info.Name,
                Type = info.ParameterType
            };
        }

        private List<CheatCommandSettings> GetCheatMethods(BaseCheatHandler handler)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo[] methods = handler.GetType().GetMethods(flags);

            return methods
                .Where(method => method.GetCustomAttribute<CheatCommandAttribute>() != null)
                .Select(method =>
                {
                    var cheatCommand = method.GetCustomAttribute<CheatCommandAttribute>();
                    return new CheatCommandSettings
                    {
                        Name = cheatCommand.Name,
                        GroupName = cheatCommand.GroupName,
                        Info = method,
                        Handler = handler
                    };
                })
                .ToList();
        }

        private bool TryGetCheatCommand(string name, out CheatCommandSettings command)
        {
            command = _handlers
                .SelectMany(pair => pair.Value)
                .FirstOrDefault(pair => pair.Name == name);
            
            return command != null;
        }

        private object GetDefault(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}
