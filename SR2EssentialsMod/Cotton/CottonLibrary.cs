using System;
using System.Linq;
using Cotton;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Prism;
using SR2E.Prism.Data;
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
    internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }

    
    public static Dictionary<string, List<Action>> onSceneLoaded = new Dictionary<string, List<Action>>();
    
    internal static List<Action<DirectedActorSpawner>> executeOnSpawnerAwake = new List<Action<DirectedActorSpawner>>();
    
    internal static List<Spawning.ReplacementSpawnerData> spawnerReplacements = new List<Spawning.ReplacementSpawnerData>();

    internal static Dictionary<string, IdentifiableTypeGroup> customGroups = new Dictionary<string, IdentifiableTypeGroup>();
    


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



    
    

}