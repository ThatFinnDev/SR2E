using System;
namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    
    internal static List<Action<DirectedActorSpawner>> executeOnSpawnerAwake = new List<Action<DirectedActorSpawner>>();
    
    internal static List<Spawning.ReplacementSpawnerData> spawnerReplacements = new List<Spawning.ReplacementSpawnerData>();

    

    

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