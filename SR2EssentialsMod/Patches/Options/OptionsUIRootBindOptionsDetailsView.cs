using Il2CppMonomiPark.SlimeRancher.UI.Options;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(OptionsUIRoot), nameof(OptionsUIRoot.BindOptionsDetailsView))]
internal static class OptionsUIRootBindOptionsDetailsView
{
    static Exception Finalizer(Exception __exception)
    {
        if (!InjectOptionsButtons.HasFlag()) return __exception;
        return null;
    }
}