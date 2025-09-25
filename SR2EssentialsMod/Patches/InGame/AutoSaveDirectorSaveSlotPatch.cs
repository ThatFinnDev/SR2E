using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppSystem.Reflection;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(AutoSaveDirectorConfiguration), nameof(AutoSaveDirectorConfiguration.SaveSlotCount), MethodType.Getter)]
public class AutoSaveDirectorSaveSlotPatch
{
    public static void Prefix(AutoSaveDirectorConfiguration __instance, ref int __result)
    {
        __result = SAVESLOT_COUNT.Get();
    }
    public static void Postfix(AutoSaveDirectorConfiguration __instance, ref int __result)
    {
        __result = SAVESLOT_COUNT.Get();
    }
}
[HarmonyPatch(typeof(AutoSaveDirectorConfiguration), nameof(AutoSaveDirectorConfiguration._saveSlotCount), MethodType.Getter)]
public class AutoSaveDirectorSaveSlotPatch2
{
    public static void Prefix(AutoSaveDirectorConfiguration __instance, ref int __result)
    {
        __result = SAVESLOT_COUNT.Get();
    }
    public static void Postfix(AutoSaveDirectorConfiguration __instance, ref int __result)
    {
        __result = SAVESLOT_COUNT.Get();
    }
}
