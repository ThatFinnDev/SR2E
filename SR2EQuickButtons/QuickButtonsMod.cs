using Il2Cpp;
using MelonLoader;
using SR2E;
using SR2E.Library;
using SR2E.Library.Buttons;
using SR2QuickButtons;
using UnityEngine.Localization;

[assembly: MelonInfo(typeof(QuickButtonsMod), "Quick Buttons", "1.0.0", "Gold Tarr Rancher")]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
namespace SR2QuickButtons
{

    public class QuickButtonsMod : SR2EMod
    {
        private static QuickButtonsMod debugInstance;
        internal SavableButtons buttonData;
        public override void OnSystemSceneLoaded()
        {
            debugInstance = this;
            SR2Console.RegisterCommands(new SR2CCommand[]
            {

            });
        }

        public override void SaveDirectorLoaded()
        {
            var path = Path.Combine(gameContext.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath, "QuickButtons.sr2e");
            if (File.Exists(path))
            {
                string dataJSON = File.ReadAllText(path);
                buttonData = Newtonsoft.Json.JsonConvert.DeserializeObject<SavableButtons>(dataJSON);
            }
            else
            {
                File.Create(path);
                buttonData = new SavableButtons();
            }
            if (buttonData.mainMenuButtons.Count != 0)
                foreach (var button in buttonData.mainMenuButtons)
                    new CustomMainMenuButton(LibraryUtils.AddTranslation(button.Label.Replace('_', ' '), $"l.quick_button_{button.Label.ToLower()}", "UI"), null, button.Index, () => SR2Console.ExecuteByString(button.Command));
        }
        public override void OnApplicationQuit()
        {
            var path = Path.Combine(gameContext.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath, "QuickButtons.sr2e");
            using (var writer = new StreamWriter(path))
            {
                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(buttonData, Newtonsoft.Json.Formatting.Indented));
                writer.Dispose();
            }
        }
    }
}