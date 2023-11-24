using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.KFC;
using Il2CppMonomiPark.SlimeRancher.Event.Query;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.MovementAndLookTypes;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using MelonLoader;
using UnityEngine;

namespace SR2E
{
    internal class HarmonyPatches
    {
        //test for adding unlock all portals command
        /*
            [HarmonyPatch(typeof(QueryToggle))]
            [HarmonyPatch("Awake")]
            public static class QueryTogglePatch
            {
                public static void Postfix(QueryToggle __instance)
                {
                    for (int i = 0; i < __instance.transform.childCount; i++)
                    {
                        if (__instance.transform.GetChild(i).gameObject.name == "TeleportTrigger")
                        {
                            __instance.transform.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                }
            }
        */
    }
}