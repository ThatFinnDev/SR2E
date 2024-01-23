using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using UnityEngine.Localization;

namespace SR2E.Patches;

[HarmonyPatch(typeof(LocalizedVersionText), nameof(LocalizedVersionText.OnEnable))]
public static class SR2LocalizedVersionTextPatch
{
    public static void Postfix(LocalizedVersionText __instance)
    {
        try
        {
            TextMeshProUGUI versionLabel = __instance.GetComponent<TextMeshProUGUI>();
            versionLabel.text = "Melonloader 0.6.2\n" + versionLabel.text+"\n\n\n";
        }
        catch { }

    }
}