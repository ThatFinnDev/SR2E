using Il2CppMonomiPark.SlimeRancher.ErrorHandling;
using Starlight.Managers;

namespace Starlight.Patches.Saving.Fixer;

[HarmonyPriority(-99999999)]
[HarmonyPatch(typeof(PopupErrorHandler), nameof(PopupErrorHandler.HandleErrorWithUserResolution))]
internal static class SaveFixerPopupErrorHandlerPatch
{
    internal static bool Prefix() => (StarlightEntryPoint.disableFixSaves && !StarlightCounterGateManager.srleActive);
}