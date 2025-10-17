using System;
using CottonLibrary.Save;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.UI;
using UnityEngine.Localization;
using Il2CppMonomiPark.SlimeRancher.Weather;
using Il2Cpp;
using UnityEngine;

namespace CottonLibrary;

public static partial class Library
{
    internal static List<Action<DirectedActorSpawner>> executeOnSpawnerAwake = new List<Action<DirectedActorSpawner>>();
    
    internal static List<ReplacementSpawnerData> spawnerReplacements = new List<ReplacementSpawnerData>();
    
    internal static List<IdentifiableTypeGroup> customGroups = new List<IdentifiableTypeGroup>();
    
    internal static Dictionary<string, IdentifiableType> savedIdents = new Dictionary<string, IdentifiableType>();


    internal static List<SR2EExpansionV2> mods = new List<SR2EExpansionV2>();


    internal static Dictionary<IdentifiableType, ModdedMarketData> marketData =
        new Dictionary<IdentifiableType, ModdedMarketData>(0);

    internal static Dictionary<PlortEntry, bool> marketPlortEntries =
        new Dictionary<PlortEntry, bool>();

    internal static List<IdentifiableType> removeMarketPlortEntries = new List<IdentifiableType>();
    internal static GameObject rootOBJ;

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


    public static GameObject? Get(string name) => Get<GameObject>(name);
    
    internal static List<Action> createLargoActions = new List<Action>();
    
    private static List<string> largoCombos = new List<string>();

    public static ModdedV01 moddedSaveData;
}
