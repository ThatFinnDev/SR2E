using CottonLibrary;
using Il2CppMonomiPark.SlimeRancher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CottonLibrary.Library;

using HarmonyLib;
using Il2Cpp;
namespace CottonLibrary.Patches;

/*
[LibraryPatch()]
[HarmonyPatch(typeof(SavedGame), nameof(SavedGame.Load))]
public static class SaveLoadedPatch
{
    public static void Postfix(SavedGame __instance)
    {
        foreach (CottonMod lib in mods)
        {
            lib.OnSavedGameLoaded();
        }

        foreach (var ident in savedIdents)
        {
            if (!GameContext.Instance.AutoSaveDirector.identifiableTypes._runtimeObject._memberTypes.Contains(ident.Value))
                GameContext.Instance.AutoSaveDirector.identifiableTypes._runtimeObject._memberTypes.Add(ident.Value);
        }
    }
}
[LibraryPatch()]
[HarmonyPatch(typeof(SavedGame), nameof(SavedGame.Save))]
public static class GameSavedPatch
{
    public static void Prefix(SavedGame __instance)
    {
        foreach (CottonMod lib in mods)
        {
            lib.PreGameSaving();
        }

        foreach (var ident in savedIdents)
        {
            INTERNAL_SetupSaveForIdent(ident.Key, ident.Value);
        }
    }
}
*/