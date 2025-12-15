using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.UI.Loading;
using SR2E.Storage;

namespace SR2E.Patches.Dev;

/*
[DevPatch()]
[HarmonyPatch(typeof(LoadingScreenView), nameof(LoadingScreenView.Awake))]
internal static class LoadingScreenViewPatch
{
    public static bool done = false;
    public static void Prefix(LoadingScreenView __instance)
    {
        if (done) return;
        var instance = Object.Instantiate(__instance.gameObject, __instance.transform.parent);
        instance.SetActive(false);
        instance.name = "LoadingScreenViewBak";
        Object.DontDestroyOnLoad(instance);
        done = true;
    }

}*/