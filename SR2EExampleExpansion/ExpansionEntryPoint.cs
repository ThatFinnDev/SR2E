using Il2CppMonomiPark.SlimeRancher;
using SR2E.Expansion;

namespace SR2EExampleExpansion
{
    public static class BuildInfo
    {
        public const string Name = "SR2EExampleExpansion"; // Name of the Expansion.  (MUST BE SET)
        public const string Description = "Test Expansion for SR2E"; // Description for the Expansion.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Expansion.  (MUST BE SET)
        public const string Company = null; // Company that made the Expansion.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Expansion.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Expansion.  (Set as null if none)
    }

    public class ExpansionEntryPoint : SR2EExpansionV1
    {
        public override void OnNormalInitializeMelon()
        {
            AddLanguages(LoadTextFile("SR2EExampleExpansion.translations.csv"));
        }

        public override void OnSR2FontLoad()
        {
            
        }
        public override void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector)
        {
            
        }
        public override void SaveDirectorLoaded(AutoSaveDirector autoSaveDirector)
        {
            
        }

        public override void LoadCommands()
        {
            // Used to register commands
            // SR2EConsole.RegisterCommand(new Command());
        }

        public override void OnGameContext(GameContext gameContext)
        {
            
        }

        public override void OnSystemContext(SystemContext systemContext)
        {
            
        }
    }

}