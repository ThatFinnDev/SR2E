using Il2CppMonomiPark.SlimeRancher.UI.Options;
using SR2E.Storage;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(OptionsUIRoot), nameof(OptionsUIRoot.Start))]
internal static class OptionsUIRootStartPatch
{
    //"OptionsConfiguration_MainMenu"
    //"OptionsConfiguration_InGame"
    public static void Prefix()
    {
        if (!InjectOptionsButtons.HasFlag()) return;
        if(SR2EEntryPoint.mainMenuLoaded) SR2EOptionsButtonManager.LoadCustomOptionsButtons("OptionsConfiguration_MainMenu");
        else if(inGame) SR2EOptionsButtonManager.LoadCustomOptionsButtons("OptionsConfiguration_InGame");
    }
}
