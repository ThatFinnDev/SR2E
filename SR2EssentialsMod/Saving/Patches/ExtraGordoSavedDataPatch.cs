﻿/*
using SR2E.Saving;
using SR2E;

[HarmonyPatch(typeof(GordoEat), nameof(GordoEat.Start))]
internal static class ExtraGordoSavedDataPatch
{
    internal static void Postfix(GordoEat __instance)
    {
        if (SR2EEntryPoint.debugLogging)
            SR2EConsole.SendMessage($"debug log gordo {__instance.gameObject.name}");
        if(__instance.gameObject.GetComponent<SR2EGordoDataSaver>()==null)
            __instance.gameObject.AddComponent<SR2EGordoDataSaver>();
    }
}*/
//Broken as of SR2 0.6.0