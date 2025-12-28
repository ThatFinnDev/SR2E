using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.Options.Fixer;

[HarmonyPatch(typeof(OptionsDirector), nameof(OptionsDirector.PushSettings))]
internal static class OptionsDirectoryPushSettingsPatch
{
    internal static void Postfix(OptionsV02 optionsData) => Prefix(optionsData);
    internal static void Prefix(OptionsV02 optionsData)
    {
        try
        {
            foreach (var entry in optionsData.OptionItems.ToNetList())
            {
                if(entry!=null)
                    if (entry.PersistenceKey.StartsWith("setting.sr2eexclude"))
                        if(optionsData.OptionItems.Contains(entry))
                            optionsData.OptionItems.Remove(entry);
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }
}