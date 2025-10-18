using System;
using System.Linq;
using Cotton;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Cotton.Enums;
using UnityEngine.Localization;
namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        var pair = onSceneLoaded.FirstOrDefault(x => sceneName.Contains(x.Key));

        if (pair.Value != null)
            foreach (var action in pair.Value)
                action();
    }
    
    
    public static Dictionary<string, List<Action>> onSceneLoaded = new Dictionary<string, List<Action>>();
    
    internal static List<Action<DirectedActorSpawner>> executeOnSpawnerAwake = new List<Action<DirectedActorSpawner>>();
    
    internal static List<Spawning.ReplacementSpawnerData> spawnerReplacements = new List<Spawning.ReplacementSpawnerData>();
    
    internal static List<IdentifiableTypeGroup> customGroups = new List<IdentifiableTypeGroup>();
    
    internal static Dictionary<string, IdentifiableType> savedIdents = new Dictionary<string, IdentifiableType>();

    internal static Dictionary<IdentifiableType, ModdedMarketData> marketData =
        new Dictionary<IdentifiableType, ModdedMarketData>(0);

    internal static Dictionary<PlortEntry, bool> marketPlortEntries =
        new Dictionary<PlortEntry, bool>();

    internal static List<IdentifiableType> removeMarketPlortEntries = new List<IdentifiableType>();

    public static IdentifiableTypeGroup? slimes;
    public static IdentifiableTypeGroup? plorts;
    public static IdentifiableTypeGroup? largos;
    public static IdentifiableTypeGroup? baseSlimes;
    public static IdentifiableTypeGroup? food;
    public static IdentifiableTypeGroup? meat;
    public static IdentifiableTypeGroup? veggies;
    public static IdentifiableTypeGroup? fruits;
    public static IdentifiableTypeGroup? nectar;
    public static IdentifiableTypeGroup? crafts;
    public static IdentifiableTypeGroup? chicks;
    public static GameObject? player;

    // public enum VanillaPediaEntryCategories { TUTORIAL, SLIMES, RESOURCES, WORLD, RANCH, SCIENCE, WEATHER }

    public static SlimeDefinitions? slimeDefinitions => gameContext.SlimeDefinitions; 
    

    private static SlimeAppearanceDirector _mainAppearanceDirector;

    public static SlimeAppearanceDirector mainAppearanceDirector
    {
        get
        {
            if (_mainAppearanceDirector == null)
                _mainAppearanceDirector = Get<SlimeAppearanceDirector>("MainSlimeAppearanceDirector");
            return _mainAppearanceDirector;
        }
        set { _mainAppearanceDirector = value; }
    }


    /// <summary>
    /// The key for this is $"{table}__{key}"
    /// </summary>
    internal static Dictionary<string, LocalizedString> existingTranslations =
        new Dictionary<string, LocalizedString>();


    
    internal static List<Action> createLargoActions = new List<Action>();
    
    internal static List<string> largoCombos = new List<string>();

}