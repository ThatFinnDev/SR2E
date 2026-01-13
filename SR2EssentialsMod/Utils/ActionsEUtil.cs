using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace SR2E.Utils;

public static class ActionsEUtil
{
    internal static Dictionary<Action, int> actionCounter = new Dictionary<Action, int>();
    public static void ExecuteInTicks(Action action, int ticks)
    {
        if (action == null) return;
        if(ticks<=0)
            try { action.Invoke(); } catch (Exception e) { MelonLogger.Error(e); }
        else actionCounter.Add((Action)(() =>
        {
            try { action.Invoke(); } catch (Exception e) { MelonLogger.Error(e); }
        }),ticks);
    }
    public static void ExecuteInSeconds(Action action, float seconds)
    {
        if(seconds<=0)
            try { action.Invoke(); } catch (Exception e) { MelonLogger.Error(e); }
        else MelonCoroutines.Start(waitForSeconds(seconds, action));
    }
    static System.Collections.IEnumerator waitForSeconds(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        try { action.Invoke(); }catch (Exception e) { MelonLogger.Error(e); }
    }
    public static void InvokeAll(this List<System.Action> actions) => actions.ForEach(action => action.Invoke());
    public static void InvokeAll(this List<Il2CppSystem.Action> actions) => actions.ForEach(action => action.Invoke());
    public static void InvokeAll(this Il2CppSystem.Collections.Generic.List<System.Action> actions) => actions.ToNetList().ForEach(action => action.Invoke());
    public static void InvokeAll(this Il2CppSystem.Collections.Generic.List<Il2CppSystem.Action> actions) => actions.ToNetList().ForEach(action => action.Invoke());
    public static void InvokeAll(this System.Action[] actions) => actions.ToNetList().ForEach(action => action.Invoke());
    public static void InvokeAll(this Il2CppSystem.Action[] actions) => actions.ToNetList().ForEach(action => action.Invoke());
    public static void InvokeAll(this Il2CppReferenceArray<Il2CppSystem.Action> actions) => actions.ToNetList().ForEach(action => action.Invoke());
    public static Il2CppSystem.Action ToIl2CppAction(this System.Action action) => action;
    public static Il2CppSystem.Action ToNetAction(this System.Action action) => action;

}