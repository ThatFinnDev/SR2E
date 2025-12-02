using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Storage;

namespace SR2E.Patches.Dev;


[DevPatch()]
[HarmonyPatch(typeof(Bootstrapper), nameof(Bootstrapper.Start))]
internal static class BootstrapperPatch
{
    public static bool done = false;
    public static void Prefix(Bootstrapper __instance)
    {
        if (done) return;
        var instance = Object.Instantiate(__instance.gameObject, __instance.transform.parent);
        instance.SetActive(false);
        instance.name = "BootstrapperBak";
        Object.DontDestroyOnLoad(instance);
        done = true;
    }

}