using Il2CppMonomiPark.SlimeRancher.UI.Options;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(OptionsUIRoot), nameof(OptionsUIRoot.ApplyChanges))]
internal static class OptionsUIRootApplyPatch
{
    internal static int realMasterTextureLimit = 0;
    internal static int customMasterTextureLimit = -1;

    public static void Apply()
    {
        if (customMasterTextureLimit == -1)
        {
            QualitySettings.masterTextureLimit = realMasterTextureLimit;
        }
        else
        {
            QualitySettings.masterTextureLimit = customMasterTextureLimit;
        }
    }

    public static void Postfix()
    {
        realMasterTextureLimit = QualitySettings.masterTextureLimit;
        Apply();
    }
}