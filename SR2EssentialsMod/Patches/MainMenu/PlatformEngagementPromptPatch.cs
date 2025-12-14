using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(PlatformEngagementPrompt), nameof(PlatformEngagementPrompt.Start))]
internal static class PlatformEngagementPromptPatch
{
    internal static bool hasRegistered = false;
    internal static void Postfix(PlatformEngagementPrompt __instance)
    {
        __instance.EngagementPromptTextUI.SetActive(false);
        __instance.OnInteract(new InputAction.CallbackContext());
        __instance.StartupClick = null;
        hasRegistered = false;
        InputSystem.onAnyButtonPress.CallOnce(
            (System.Action<InputControl>)((ee) =>
            {
                if(hasRegistered) return;
                hasRegistered = true;
                ExecuteInTicks(() =>
                {
                    if (SR2EEntryPoint.mainMenuLoaded)
                    {
                        GetAnyInScene<MainMenuLandingRootUI>().Awake();
                    }
                }, 1);
                
            }));
    }
}
