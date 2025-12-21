using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.CommonControls;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(StaticItemsDataListView), nameof(StaticItemsDataListView.Start))]
internal static class StaticItemsDataListViewPatch
{
    internal static void Prefix(StaticItemsDataListView __instance)
    {
        var name = __instance.name;
        var parentName = __instance.transform.parent == null ? null : __instance.transform.parent.name;
        if (
            name != "LayoutGroup_Right (MainMenuButtons)" ||
            (name!="SubMenuList"&&parentName=="LayoutGroup_Middle (ExtraButtonPrompts)")
            ) return;
        if (!__instance.transform.GetChild(0).name.Contains("MainMenu")) return;
        for (int i = 0; i < 20; i++)
            GameObject.Instantiate(__instance.transform.GetChild(0), __instance.transform);
    }
}