using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;

namespace SR2E.Patches.InGame;
/*
 
 BROKEN SINCE 1.0.0!
 Needs fixing

[HarmonyPatch(typeof(SavedGame), nameof(SavedGame.Push), typeof(GameModel))]
public static class LoadPatch
{   
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
[HarmonyPatch(typeof(SavedGame), nameof(SavedGame.Pull), typeof(GameModel))]
public static class SavePatch
{
    [HarmonyFinalizer]
    static Exception Pull(Exception __exception)
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
}*/