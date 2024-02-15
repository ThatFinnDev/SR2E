using SR2E.Saving;
using SR2E;
using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.BeginLoad))]
public static class AutoSaveDirectorLoadPatch
{
    public static string loadPath => GameContext.Instance.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath;

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
                SR2EConsole.SendWarning("Failed to load SR2E save data, creating new");
                SR2EConsole.SendWarning($"Developer error: {ex}");
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
