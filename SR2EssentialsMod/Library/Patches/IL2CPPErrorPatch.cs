using HarmonyLib;
using MelonLoader;
using SR2E.Storage;

namespace CottonLibrary.Patches;

[LibraryPatch()]
[HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
public class IL2CPPErrorPatch
{
    public static bool Prefix(System.Exception ex)
    {
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;                               
    }
}