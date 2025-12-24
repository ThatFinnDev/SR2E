namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(WorldPopulator._PopulateRanch_d__3), "MoveNext")]
internal class WorldPopulatorErrorPatchPopulateRanch
{
    public static void Prefix(WorldPopulator._PopulateRanch_d__3 __instance)
    {
        if (!ShowWorldPopulatorErrors.HasFlag()) return;
        if(IgnoreWorldPopulatorErrors.HasFlag())
            __instance.onFail = new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
        else
            __instance.onFail += new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
    }
}
[HarmonyPatch(typeof(WorldPopulator._Populate_d__2), "MoveNext")]
internal class WorldPopulatorErrorPatchPopulate
{
    public static void Prefix(WorldPopulator._Populate_d__2 __instance)
    {
        if (!ShowWorldPopulatorErrors.HasFlag()) return;
        if(IgnoreWorldPopulatorErrors.HasFlag())
            __instance.onFail = new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
        else
            __instance.onFail += new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
    }
}
[HarmonyPatch(typeof(WorldPopulator._PopulateActors_d__5), "MoveNext")]
internal class WorldPopulatorErrorPatchPopulateActors
{
    public static void Prefix(WorldPopulator._PopulateActors_d__5 __instance)
    {
        if (!ShowWorldPopulatorErrors.HasFlag()) return;
        if(IgnoreWorldPopulatorErrors.HasFlag())
            __instance.onFail = new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
        else
            __instance.onFail += new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
    }
}
[HarmonyPatch(typeof(WorldPopulator._PopulateDrones_d__6), "MoveNext")]
internal class WorldPopulatorErrorPatchPopulateDrones
{
    public static void Prefix(WorldPopulator._PopulateDrones_d__6 __instance)
    {
        if (!ShowWorldPopulatorErrors.HasFlag()) return;
        if(IgnoreWorldPopulatorErrors.HasFlag())
            __instance.onFail = new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
        else
            __instance.onFail += new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
    }
}
[HarmonyPatch(typeof(WorldPopulator._PopulateGadgets_d__4), "MoveNext")]
internal class WorldPopulatorErrorPatchPopulateGadgets
{
    public static void Prefix(WorldPopulator._PopulateGadgets_d__4 __instance)
    {
        if (!ShowWorldPopulatorErrors.HasFlag()) return;
        if(IgnoreWorldPopulatorErrors.HasFlag())
            __instance.onFail = new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
        else
            __instance.onFail += new System.Action<Il2CppSystem.Exception>((exception) =>
            {
                MelonLogger.Error($"Coroutine exception in WorldPopulator.PopulateRanch:\n{exception.ToString()}");
            });
    }
}