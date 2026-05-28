using Il2CppMonomiPark.SlimeRancher.Slime;
using Starlight.Storage;

namespace Starlight.Prism.Patches;

[PrismPatch,FeatureFlagDependentPatch(SupportRadiant)]
[HarmonyPatch(typeof(RadiantSlimeDirector),nameof(RadiantSlimeDirector.Awake))]
internal static class RadiantSlimeDirectorPatch
{
    public static void Postfix(RadiantSlimeDirector __instance)
    {
        var oldList = __instance._radiantSlimeConfig._radiantShuffleBagSizes.ToNetList();
        var newList = oldList.ToNetList();
        foreach (var pair in PrismShortcuts.PrismBaseSlimes)
            if (!pair.Value.GetIsNative()&&pair.Value.GetSlimeAppearanceRadiant()!=null)
            {
                var contains = false;
                foreach (var entry in oldList)
                    if(entry!=null && entry.Slime == pair.Value.SlimeDefinition)
                    {
                        contains = true;
                        break;
                    }
                if (!contains)
                    newList.Add(new RadiantSlimeConfig.RadiantShuffleBagConfigEntry() { BagSize = pair.Value.NonNativeBagSize,Slime = pair.Value });
            }
        __instance._radiantSlimeConfig._radiantShuffleBagSizes = newList.ToIl2CppArray();
    }
}