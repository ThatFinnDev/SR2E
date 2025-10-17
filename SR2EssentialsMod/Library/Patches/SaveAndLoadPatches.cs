using System.IO;
using System.Linq;
using CottonLibrary;
using CottonLibrary.Save;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SR2E.Storage;
using UnityEngine;
using BinaryReader = System.IO.BinaryReader;
using FileMode = System.IO.FileMode;
using FileStream = System.IO.FileStream;

namespace CottonLibrary.Patches;


[LibraryPatch()]
[HarmonyPatch]
public static class LoadPatch
{
    [LibraryPatch()]
    [HarmonyPostfix, HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Load), typeof(string), typeof(string), typeof(bool))]
    static void ModdedLoad(AutoSaveDirector __instance, string gameName, string saveName, bool reloadAllCoreScenes)
    {
        using (FileStream fs =
               new FileStream(__instance._storageProvider.Cast<FileStorageProvider>().savePath + "/" +  saveName + ".mod", FileMode.OpenOrCreate))
        {
            var save = new ModdedV01();
            save.Writer = new BinaryWriter(fs);
            try
            {
                save.ReadData();
            }
            catch {}
            Callbacks.Invoke_onModdedLoad(save);
            moddedSaveData = save;
        }
    }
    
    //[LibraryPatch()]
    //[HarmonyFinalizer, HarmonyPatch(typeof(SavedGame), "Push", typeof(GameModel))]
    static Exception CatchPush(Exception __exception)
    {
        if (__exception == null) return null;

        MelonLogger.Error($"Error occured while pushing saved game!\nThe error: {__exception}\n\nContinuing!");
        
        return null;
    }
}
public static class SavePatch
{
    [LibraryPatch()]
    [HarmonyPrefix, HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
    static void ModdedSave(AutoSaveDirector __instance)
    {
        using (FileStream fs =
               new FileStream(__instance._storageProvider.Cast<FileStorageProvider>().savePath + "/" + __instance._currentGameMetadata.value.GameName + ".mod", FileMode.CreateNew))
        {
            var save = moddedSaveData;
            save.Writer = new BinaryWriter(fs);
            
            Callbacks.Invoke_onModdedSave(save);
            
            try
            {
                save.WriteData();
            }
            catch {}
        }
    }
}