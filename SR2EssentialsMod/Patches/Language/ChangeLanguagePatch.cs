using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using Il2CppSystem.Globalization;

namespace SR2E.Patches.Language;

[HarmonyPatch(typeof(LocalizationDirector), nameof(LocalizationDirector.SetLocale))]
public class ChangeLanguagePatch
{
    public static void Prefix(LocalizationDirector __instance, UnityEngine.Localization.Locale locale)
    {
        var code = locale.Formatter.Cast<CultureInfo>()._name;
        
        if (languages.TryGetValue(code, out var _))
            LoadLanguage(code);
        else
            LoadLanguage("en");

        sr2etosrlanguage.Clear();
        foreach (var str in sr2eReplaceOnLanguageChange)
            AddTranslationFromSR2E(str.Key, str.Value.Item1, str.Value.Item2);
    }
}