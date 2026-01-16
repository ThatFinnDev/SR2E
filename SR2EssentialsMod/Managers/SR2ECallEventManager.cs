using System.Reflection;
using SR2E.Enums;
using SR2E.Storage;

namespace SR2E.Managers;

internal static class SR2ECallEventManager
{
    private static Dictionary<CallEvent, List<MethodInfo>> _registry = new();

    internal static void LoadAssemblies(List<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        foreach (var type in assembly.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attr = method.GetCustomAttribute<CallOnAttribute>();
                if (attr == null) continue;

                if (!method.IsStatic)
                {
                    MelonLogger.Error($"{method.Name} in {type.Name} has [CallOn], but is not static.");
                    continue;
                }

                if (method.ReturnType != typeof(void))
                {
                    MelonLogger.Error($"{method.Name} in {type.Name} has [CallOn], but does not return void.");
                    continue;
                }

                int enumVal = (int)attr.callEvent;

                if (enumVal > 0 && method.GetParameters().Length > 0)
                {
                    MelonLogger.Error($"{method.Name} uses event {attr.callEvent} without parameters.");
                    continue;
                }

                if (!_registry.ContainsKey(attr.callEvent))
                    _registry[attr.callEvent] = new List<MethodInfo>();

                _registry[attr.callEvent].Add(method);
            }
        }

    }

    /// Executes positive enum functions (No arguments)
    internal static void ExecuteStandard(CallEvent level)
    {
        if (!_registry.TryGetValue(level, out var methods)) return;

        foreach (var method in methods)
        {
            try
            {
                method.Invoke(null, null);
            }
            catch (Exception ex) { MelonLogger.Error($"Exception in {method.Name}: {ex.InnerException?.Message ?? ex.Message}"); }
        }
    }

    /// Executes negative enum functions with custom argument
    internal static void ExecuteWithArgs(CallEvent callEvent, params (string, object)[] arguments)
    {
        if (!_registry.TryGetValue(callEvent, out var methods)) return;

        var providedArgs = new Dictionary<string, object>();
        if(arguments!=null)
            foreach (var pair in arguments)
                providedArgs.Add(pair.Item1,pair.Item2);
        
        foreach (var method in methods)
        {
            try
            {
                var parameters = method.GetParameters();
                object[] callArgs = new object[parameters.Length];
                bool canCall = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    if (providedArgs.TryGetValue(p.Name, out var val) && p.ParameterType.IsInstanceOfType(val))
                    { callArgs[i] = val; }
                    else
                    {
                        MelonLogger.Error($"Unsupported or missing argument: '{p.Name}' of type {p.ParameterType.Name} in method {method.Name}");
                        canCall = false;
                        break;
                    }
                }

                if (canCall) method.Invoke(null, callArgs);
            }
            catch (Exception ex) { MelonLogger.Error($"Exception in {method.Name}: {ex.InnerException?.Message ?? ex.Message}"); }
        }
    }
}