using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(SRButton), nameof(SRButton.Awake))]
internal static class GetSR2FontPatch
{
    internal static void Postfix(SRButton __instance)
    {
        if (__instance.GetComponent<MainMenuButton>() != null)
        {
            if (SR2EEntryPoint.SR2Font != null) return;
            TextMeshProUGUI label = __instance.gameObject.GetObjectRecursively<TextMeshProUGUI>("Button_Label");
            if (label != null) SR2EEntryPoint.SR2Font = label.font;
            SR2EEntryPoint.SetupFonts();
        }
        
    }
}