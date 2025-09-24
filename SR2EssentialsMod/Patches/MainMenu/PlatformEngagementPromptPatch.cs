using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.InputSystem;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(PlatformEngagementPrompt), nameof(PlatformEngagementPrompt.Start))]
internal static class PlatformEngagementPromptPatch
{
    internal static void Postfix(PlatformEngagementPrompt __instance)
    {
        __instance.EngagementPromptTextUI.SetActive(false);
        __instance.OnInteract(new InputAction.CallbackContext());
    }
}
