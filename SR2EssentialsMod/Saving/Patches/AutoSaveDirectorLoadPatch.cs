using SR2E.Saving;
using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.BeginLoad))]
public static class AutoSaveDirectorLoadPatch
{
    public static string loadPath => GameContext.Instance.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath;

    public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName, Il2CppSystem.Action onError)
    {
        string[] split = saveName.Split("_");
        string path = Path.Combine(loadPath, split[0] + "_" + split[1] + ".sr2ev2");
        if (File.Exists(path))
        {
            try
            {
                SR2ESavableDataV2.LoadFromStream(new FileStream(path, FileMode.Open));
                SR2ESavableDataV2.currPath = path;
                SR2ESavableDataV2.Instance.dir = $"{loadPath}\\";
                SR2ESavableDataV2.Instance.gameName = gameName;
            }
            catch (Exception ex)
            {
                SR2EConsole.SendWarning("Failed to load SR2E save data, creating new");
                SR2EConsole.SendWarning($"Developer error: {ex}");
                
                //File.Create(path);
                new SR2ESavableDataV2();
                SR2ESavableDataV2.currPath = path;
                SR2ESavableDataV2.Instance.dir = $"{loadPath}\\";
                SR2ESavableDataV2.Instance.gameName = gameName;
            }
        }
        else
        {
            //File.Create(path);
            new SR2ESavableDataV2();
            SR2ESavableDataV2.currPath = path;
            SR2ESavableDataV2.Instance.dir = $"{loadPath}\\";
            SR2ESavableDataV2.Instance.gameName = gameName;
        }


    }
}
