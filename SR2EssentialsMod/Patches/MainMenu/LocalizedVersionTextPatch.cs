using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using UnityEngine.Localization;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(LocalizedVersionText), nameof(LocalizedVersionText.OnEnable))]
public static class LocalizedVersionTextPatch
{
    public static void Postfix(LocalizedVersionText __instance)
    {
        if (DisableLocalizedVersionPatch.HasFlag()) return;
        try
        {
            TextMeshProUGUI versionLabel = __instance.GetComponent<TextMeshProUGUI>();
            if (SR2EEntryPoint.newVersion != null)
                if(SR2EEntryPoint.newVersion!=BuildInfo.Version)
                    versionLabel.text = $"New SR2E version available: {SR2EEntryPoint.newVersion}\n{versionLabel.text}";
            versionLabel.text = "MelonLoader "+SR2EEntryPoint.MLVERSION+"\n" + versionLabel.text+"\n\n\n";
                
        }
        catch { }

    }
}