﻿using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Patches.MainMenu;

<<<<<<< HEAD
[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.Init))]
public static class SaveGameRootUIPatch
{
    public static void Prefix()
    {
        SR2EEntryPoint.SaveCountChanged = false;
=======
[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.FocusUI))]
internal static class SaveGameRootUIPatch
{
    internal static void Postfix(SaveGamesRootUI __instance)
    {
        SR2EEntryPoint.baseUIAddSliders.Add(__instance);
>>>>>>> experimental
    }
}