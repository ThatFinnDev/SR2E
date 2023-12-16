using Il2CppMonomiPark.SlimeRancher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.Patches;

[HarmonyPatch(typeof(SavedGame), "Load")]
public static class SaveLoadedPatch
{
    public static void Postfix(SavedGame __instance)
    {
        foreach (SR2EMod lib in mods)
        {
            lib.OnSavedGameLoaded();
        }
    }
}