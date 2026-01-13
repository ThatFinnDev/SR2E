using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.Options.Fixer;

[HarmonyPatch(typeof(OptionsModel), nameof(OptionsModel.Pull))]
internal static class OptionsModelPullPatch
{
    internal static void Postfix(OptionsV02 persistence) => Prefix(persistence);
    internal static void Prefix(OptionsV02 persistence)
    {
        try
        {
            foreach (var entry in persistence.OptionItems.ToNetList())
            {
                if(entry!=null)
                    if (entry.PersistenceKey.StartsWith("setting.sr2eexclude"))
                        if(persistence.OptionItems.Contains(entry))
                            persistence.OptionItems.Remove(entry);
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }
}