using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Patches;

[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.Awake))]
public static class SaveGameRootUIPatch
{
    public static void Prefix()
    {
        SR2EEntryPoint.SaveCountChanged = false;
    }
}