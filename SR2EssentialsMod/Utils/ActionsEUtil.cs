using System;

namespace SR2E.Utils;

public static class ActionsEUtil
{
    internal static Dictionary<Action, int> actionCounter = new Dictionary<Action, int>();
    public static void ExecuteInTicks(Action action, int ticks)
    {
        if (action == null) return;
        actionCounter.Add((Action)(() => { action.Invoke(); }),ticks);
    }
    public static void ExecuteInSeconds(Action action, float seconds)
    {
        MelonCoroutines.Start(waitForSeconds(seconds, action));
    }
    static System.Collections.IEnumerator waitForSeconds(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        try { action.Invoke(); }catch (Exception e) { MelonLogger.Error(e); }
    }
}