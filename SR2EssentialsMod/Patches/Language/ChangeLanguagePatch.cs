using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using Il2CppSystem.Globalization;

namespace SR2E.Patches.Language;

[HarmonyPatch(typeof(LocalizationDirector), nameof(LocalizationDirector.SetLocale))]
internal class ChangeLanguagePatch
{
    internal static void Postfix(LocalizationDirector __instance, UnityEngine.Localization.Locale locale)
    {
        var code = locale.Formatter.Cast<CultureInfo>()._name;
        
        if (languages.ContainsKey(code))
            LoadLanguage(code);
        else
            LoadLanguage("en");

        sr2etosrlanguage.Clear();
        addedTranslations.Clear();
        foreach (var str in sr2eReplaceOnLanguageChange)
        {
            var localized = AddTranslation(translation(str.Key), str.Value.Item1, str.Value.Item2);

            var original = str.Value.Item3;
            
            original.TableEntryReference = localized.TableEntryReference;
            original.TableReference = localized.TableReference;
        }
    }
}