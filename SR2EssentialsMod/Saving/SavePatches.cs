using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using System.IO;

namespace SR2E.Saving;

public static class SavePatches
{
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
    public static class ExtraSlimeSavedDataPatch
    {
        public static void Postfix(AutoSaveDirector __instance)
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
    [HarmonyPatch(typeof(GordoEat), nameof(GordoEat.Start))]
    public static class ExtraGordoSavedDataPatch
    {
        public static void Postfix(GordoEat __instance)
        {
            if (SR2EEntryPoint.debugLogging)
                SR2Console.SendMessage($"debug log gordo {__instance.gameObject.name}");
            __instance.gameObject.AddComponent<SR2EGordoDataSaver>();
        }
    }

    [HarmonyPatch(typeof(SRCharacterController), nameof(SRCharacterController.Start))]
    public static class PlayerDataLoadPatch
    {
        public static void Postfix(SRCharacterController __instance)
        {
            try
            {

                if (SR2ESavableData.Instance.playerSavedData.vacMode == VacModes.AUTO_VAC || SR2ESavableData.Instance.playerSavedData.vacMode == VacModes.AUTO_VAC)
                {
                    SR2ESavableData.Instance.playerSavedData.vacMode = VacModes.NORMAL;
                }
                NoClipCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.noclipState);
                SpeedCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.speed);
                UtilCommand.RemoteExc_PlayerSize(SR2ESavableData.Instance.playerSavedData.size);
                UtilCommand.PlayerVacModeSet(SR2ESavableData.Instance.playerSavedData.vacMode);
                SceneContext.Instance.player.GetComponent<SRCharacterController>().Velocity = Vector3Data.ConvertBack(SR2ESavableData.Instance.playerSavedData.velocity);
                SceneContext.Instance.player.GetComponent<SRCharacterController>()._gravityMagnitude = new Il2CppSystem.Nullable<float>(SR2ESavableData.Instance.playerSavedData.gravityLevel);
                if (SR2ESavableData.Instance.playerSavedData.noclipState && SR2EEntryPoint.debugLogging)
                {
                    SR2Console.SendMessage("Load noclip state debug");
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
    public static class AutoSaveDirectorSavePatch
    {
        public static void Postfix(AutoSaveDirector __instance)
        {
            if (SR2EEntryPoint.debugLogging)
            {
                MelonLogger.Msg("test");
            }

            SR2ESavableData.Instance.playerSavedData.velocity = new Vector3Data(SceneContext.Instance.player.GetComponent<SRCharacterController>().Velocity);
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
            foreach (var gordo in Resources.FindObjectsOfTypeAll<SR2EGordoDataSaver>())
            {
                try
                {
                    gordo.SaveData();
                }
                catch
                {

                }
            }
            if (SR2ESavableData.Instance.idx != AutoSaveDirector.MAX_AUTOSAVES)
            {
                SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{SR2ESavableData.Instance.idx + 1}.sr2e";
                SR2ESavableData.Instance.idx++;
            }
            else
            {
                SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{0}.sr2e";
                SR2ESavableData.Instance.idx = 0;
            }
            if (SR2EEntryPoint.debugLogging)
                SR2Console.SendWarning(SR2ESavableData.currPath);
            SR2ESavableData.Instance.TrySave();
        }
    }
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.DeleteSave))]
    public static class AutoSaveDirectorDeletePatch
    {
        public static void Prefix(AutoSaveDirector __instance, string saveName )
        {
            string path = Path.Combine(SystemContext.Instance.GetStorageProvider().Cast<FileStorageProvider>().savePath, $"{saveName}.sr2e");
            if(File.Exists(path))
                File.Delete(path);
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


        }
    }
}
