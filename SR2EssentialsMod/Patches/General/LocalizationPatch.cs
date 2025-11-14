using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using System.Collections;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(LocalizationDirector), nameof(LocalizationDirector.LoadTables))]
internal static class LocalizationDirectorLoadTablePatch
{
    internal static void Postfix(LocalizationDirector __instance)
    {
        if(InjectTranslations.HasFlag())
            MelonCoroutines.Start(LoadTable(__instance));
    }
    private static IEnumerator LoadTable(LocalizationDirector director)
    {
        WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(0.01f);
        yield return waitForSecondsRealtime;
        foreach (var keyValuePair in director.Tables)
        {
            Dictionary<string, string> dictionary;
            if (addedTranslations.TryGetValue(keyValuePair.Key, out dictionary))
            {
                foreach (KeyValuePair<string, string> keyValuePair2 in dictionary)
                {
                    keyValuePair.Value.AddEntry(keyValuePair2.Key, keyValuePair2.Value);
                }
            }
        }
        SR2EEntryPoint.CheckFallBackFont();
        yield break;
    }
}