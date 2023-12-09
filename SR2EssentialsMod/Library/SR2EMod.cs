
namespace SR2E.Library
{
    public class SR2EMod : MelonMod
    {
        public Semver.SemVersion version
        {
            get
            {
                return Info.Version;
            }
        }
        public static GameObject player { get { return LibraryUtils.player; } set { LibraryUtils.player = value; } }
        public static SystemContext systemContext { get { return LibraryUtils.systemContext; } }
        public static GameContext gameContext { get { return LibraryUtils.gameContext; } }
        public static SceneContext sceneContext { get { return LibraryUtils.sceneContext; } }
        public static SlimeDefinitions slimeDefinitions { get { return LibraryUtils.slimeDefinitions; } set { LibraryUtils.slimeDefinitions = value; } }

        public virtual void OnPlayerSceneLoaded() { }
        public virtual void OnSystemSceneLoaded() { }
        public virtual void OnGameCoreLoaded() { }
        public virtual void OnZoneCoreLoaded() { }
        public virtual void OnSavedGameLoaded() { }
        public virtual void SaveDirectorLoaded() { }
        public virtual void SaveDirectorLoading(AutoSaveDirector saveDirector) { }
        

        /*
        internal static void AddPageToPediaEntry(PediaEntry pedia, string pageText, PediaPage template)
        {
            AddTranslation(pageText, INTERNAL_CreatePediaPageKey(template.name.ToLower(), pedia), "PediaPage");
        }

        internal static string INTERNAL_CreatePediaPageKey(string prefix, PediaEntry pediaEntryName)
        {
            return "m." + prefix + "." + pediaEntryName.name.ToLower().Replace(" ", "_") + ".page." + 1.ToString();
        }

        internal static string INTERNAL_CreatePediaKey(string prefix, IdentifiableType identifiableType)
        {
            return "m." + prefix + "." + identifiableType.localizationSuffix;
        }

        internal static string INTERNAL_CreatePediaKey(string prefix, string suffix)
        {
            return "m." + prefix + "." + suffix;
        }

        */
        


    }
}
