using System;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using SR2E.Buttons;

namespace SR2E.Patches.MainMenu;


[HarmonyPatch(typeof(LoadGameBehaviorModel), nameof(LoadGameBehaviorModel.InvokeBehavior))]
internal class SR2MainMenuButtonPressPatch
{
    internal static bool Prefix(LoadGameBehaviorModel __instance)
    {
        if (__instance.Definition is CustomMainMenuItemDefinition definition)
        {
            if(definition.customAction!=null) definition.customAction.Invoke();
            return false;
        }
        if (__instance.Definition is CustomMainMenuSubItemDefinition definition2)
        {
            if(definition2.customAction!=null) definition2.customAction.Invoke();
            return false;
        }

        return true;
    }
}