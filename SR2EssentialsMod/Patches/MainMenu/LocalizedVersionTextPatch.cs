using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using UnityEngine.Localization;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(LocalizedVersionText), nameof(LocalizedVersionText.OnEnable))]
internal static class LocalizedVersionTextPatch
{
    internal static void Postfix(LocalizedVersionText __instance)
    {
        if (!EnableLocalizedVersionPatch.HasFlag()) return;
        try
        {
            TextMeshProUGUI versionLabel = __instance.GetComponent<TextMeshProUGUI>();
            if (SR2EEntryPoint.newVersion != null)
                if(SR2EEntryPoint.newVersion!=BuildInfo.Version)
                {
                    if (SR2EEntryPoint.updatedSR2E) 
                        versionLabel.text = translation("patches.localizedversionpatch.downloadedversion",SR2EEntryPoint.newVersion,versionLabel.text);
                    else versionLabel.text = translation("patches.localizedversionpatch.newversion",SR2EEntryPoint.newVersion,versionLabel.text);
                }
            versionLabel.text = translation("patches.localizedversionpatch.default",SR2EEntryPoint.MLVERSION,versionLabel.text);
                
        }
        catch { }

    }
}