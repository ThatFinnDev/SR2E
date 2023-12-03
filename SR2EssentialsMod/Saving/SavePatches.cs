using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Saving
{
    public static class SavePatches
    {
        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
        public static class ExtraSlimeSavedDataPatch
        {
            public static void Postfix(IdentifiableActor __instance)
            {
                foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableType>())
                {
                    if (ident.prefab != null)
                    {
                        var p = ident.prefab;
                        var dataSaver = p.AddComponent<SR2ESlimeDataSaver>();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SRCharacterController), nameof(SRCharacterController.Start))]
        public static class NoclipStateLoadPatch
        {
            public static void Postfix(SRCharacterController __instance)
            {
                NoClipCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.noclipState);
                if (SR2ESavableData.Instance.playerSavedData.noclipState)
                {
                    SR2Console.SendMessage("Load noclip state debug");
                }
            }
        }

        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
        public static class AutoSaveDirectorSavePatch
        {
            public static void Prefix(AutoSaveDirector __instance)
            {
                MelonLogger.Msg("test");
                foreach (var savableSlime in Resources.FindObjectsOfTypeAll<SR2ESlimeDataSaver>())
                {
                    try
                    {
                        savableSlime.SaveData();
                    }
                    catch
                    {

                    }
                }
                if (SR2ESavableData.Instance.idx != 5)
                {
                    SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{SR2ESavableData.Instance.idx + 1}.sr2e";
                    SR2ESavableData.Instance.idx++;
                }
                else
                {
                    SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{0}.sr2e";
                    SR2ESavableData.Instance.idx = 0;
                }
                MelonLogger.Warning(SR2ESavableData.currPath);
                SR2ESavableData.Instance.TrySave();
            }
        }

        [HarmonyPatch(nameof(FileStorageProvider), "savePath", MethodType.Getter)]
        public static class StorageProviderPathGrabber
        {
            static void Postfix(ref string __result)
            {
                AutoSaveDirectorLoadPatch.loadPath = __result;
            }
        }

        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.BeginLoad))]
        public static class AutoSaveDirectorLoadPatch
        {
            public static string loadPath;
            public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName, Il2CppSystem.Action onError)
            {
                if (File.Exists(Path.Combine(loadPath, $"{saveName}.sr2e")))
                {
                    try
                    {
                        SR2ESavableData.LoadFromStream(new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.Open));
                        SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                        SR2ESavableData.Instance.dir = $"{loadPath}\\";
                        SR2ESavableData.Instance.gameName = gameName;
                        SR2ESavableData.Instance.idx = int.Parse(saveName.Split('_')[2]);
                    }
                    catch (Exception ex)
                    {
                        SR2Console.SendWarning("Failed to load SR2E save data, creating new");
                        SR2Console.SendWarning($"Developer error: {ex}");
                        var stream = new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.OpenOrCreate);
                        new SR2ESavableData();
                        SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                        SR2ESavableData.Instance.dir = $"{loadPath}\\";
                        SR2ESavableData.Instance.gameName = gameName;
                        SR2ESavableData.Instance.idx = int.Parse(saveName.Split('_')[2]);
                    }
                }
                else
                {
                    var stream = new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.CreateNew);
                    new SR2ESavableData();
                    SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                    SR2ESavableData.Instance.dir = $"{loadPath}\\";
                    SR2ESavableData.Instance.gameName = gameName;
                    SR2ESavableData.Instance.idx = int.Parse(saveName.Split('_')[2]);
                }
                SR2ESlimeDataSaver.LoadData();

            }
        }
    }
}
