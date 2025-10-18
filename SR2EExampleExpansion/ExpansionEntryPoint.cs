using Il2CppMonomiPark.SlimeRancher;
using SR2E.Expansion;
using SR2E.Utils;

namespace SR2EExampleExpansion
{
    public static class BuildInfo
    {
        public const string Name = "SR2EExampleExpansion"; // Name of the Expansion. 
        public const string Description = "Test Expansion for SR2E"; // Description for the Expansion.
        public const string Author = "ThatFinn"; // Author of the Expansion.
        public const string CoAuthors = null; // CoAuthor(s) of the Expansion.  (optional, set as null if none)
        public const string Contributors = null; // Contributor(s) of the Expansion.  (optional, set as null if none)
        public const string Company = null; // Company that made the Expansion.  (optional, set as null if none)
        public const string Version = "1.0.0"; // Version of the Expansion.
        public const string DownloadLink = null; // Download Link for the Expansion.  (optional, set as null if none)
        public const string SourceCode = null; // Source Link for the Expansion.  (optional, set as null if none)
        public const string Nexus = null; // Nexus Link for the Expansion.  (optional, set as null if none)
        public const bool RequireLibrary = false; // Enable if you use Cotton
    }

    public class ExpansionEntryPoint : SR2EExpansionV2
    {
        public override void OnNormalInitializeMelon()
        {
            AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv"));
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