using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;

namespace SR2E.Patches.Saving;


[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGame))]
internal static class LoadPatch
{   
    [HarmonyFinalizer]
    static Exception Finalizer(Exception __exception)
    {
        if (__exception == null) return null;
        if (IgnoreSaveErrors.HasFlag())
        {
            MelonLogger.Error($"Error occured while pushing saved game!\nThe error: {__exception}\n\nContinuing!");
            return null;
        }
        MelonLogger.Error($"Error occured while pushing saved game!\nThe error: {__exception}");
        return __exception;
    }
}
[HarmonyPatch(typeof(GameModelPullHelpers), nameof(GameModelPullHelpers.PullGame))]
internal static class SavePatch
{
    [HarmonyFinalizer]
    static Exception Finalizer(Exception __exception)
    {
        if (__exception == null) return null;
        if (IgnoreSaveErrors.HasFlag())
        {
            MelonLogger.Error($"Error occured while pulling saved game!\nThe error: {__exception}\n\nContinuing!");
            return null;
        }
        MelonLogger.Error($"Error occured while pulling saved game!\nThe error: {__exception}");
        return __exception;
    }
}