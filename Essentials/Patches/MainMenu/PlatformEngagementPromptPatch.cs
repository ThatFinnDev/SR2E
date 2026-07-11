using System.Collections;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Starlight.Patches.MainMenu;

[HarmonyPatch(typeof(PlatformEngagementPrompt), nameof(PlatformEngagementPrompt.Start))]
internal static class PlatformEngagementPromptPatch
{
    internal static void Postfix(PlatformEngagementPrompt __instance)
    {
        if (StarlightEntryPoint.skipEngagementPrompt)
        {
            __instance.EngagementPromptTextUI.SetActive(false);
            try
            {
                __instance.OnInteract(new InputAction.CallbackContext());
            } 
            catch (Exception e) { LogError(e);}
        }
        if(ForceLoadMainMenu.HasFlag())
            StartCoroutine(Load(__instance));
    }
    
    static IEnumerator Load(PlatformEngagementPrompt __instance)
    {
        yield return new WaitForSecondsRealtime(5f);
        __instance.LoadSceneGroup();
    }
}
