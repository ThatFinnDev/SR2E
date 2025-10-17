using HarmonyLib;
using MelonLoader;
namespace CottonLibrary.Patches;

[HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
public class IL2CPPErrorPatch
{
    public static bool Prefix(System.Exception ex)
    {
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;                               
    }
}