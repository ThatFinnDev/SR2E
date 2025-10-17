using System;
using System.Linq;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using JetBrains.Annotations;
using UnityEngine;

namespace CottonLibrary;

public static partial class Library
{
    [Flags]
    public enum SpawnLocations : ushort
    {
        RainbowFields = 1 << 0,
        StarlightStand = 1 << 1,
        EmberValley = 1 << 2,
        PowderfallBluffs = 1 << 3,
        LabyrinthWaterworks = 1 << 4,
        LabyrinthLavadepths = 1 << 5,
        LabyrinthDreamland = 1 << 6,
        LabyrinthHub = 1 << 7,
        CustomArray = 1 << 8,
        All = 255
    }

    [Flags]
    public enum SpawnerTypes : byte
    {
        Slime = 1 << 0,
        Animal = 1 << 1,
    }

    public enum SpawningMode
    {
        Default,
        [Obsolete("Doesn't work!")]
        ReplacementBasedSpawning
    }

    [Flags]
    public enum RequiredConditions
    {
        None = 0,
        /// <summary>
        /// Requires an Angler Slime to meet the condition.
        /// </summary>
        RequiresAnglerSlime = 1 << 0,
        /// <summary>
        /// Requires any Angler Largo to meet the condition.
        /// </summary>
        RequiresAnglerLargo = 1 << 1,
        /// <summary>
        /// Requires only Largo Slimes to meet the condition, if there is a normal slime that can be spawned, it will NOT meet the condition.
        /// </summary>
        RequiresLargosOnly = 1 << 2,
        /// <summary>
        /// Requires only Pink Slimes or any Modded Slime to meet the condition. If there are any other slimes, including largos, the condition will NOT be met.
        /// </summary>
        RequiresOnlyPinkOrModdedSlimes = 1 << 3,
    }
    
    public static void MakeSpawnableInZones(IdentifiableType ident, DirectedActorSpawner.TimeWindow timeWindow,
        SpawnLocations zones, float weight, SpawnerTypes spawnerTargets) =>
        MakeSpawnableInZones(ident, timeWindow, zones, weight, spawnerTargets, SpawningMode.Default, RequiredConditions.None, Array.Empty<string>());

    public static void MakeSpawnableInZones(IdentifiableType ident, DirectedActorSpawner.TimeWindow timeWindow,
        SpawnLocations zones, float weight, SpawnerTypes spawnerTargets, SpawningMode mode,RequiredConditions requiredConditions) =>
        MakeSpawnableInZones(ident, timeWindow, zones, weight, spawnerTargets, mode, requiredConditions, Array.Empty<string>());

    /// <summary>
    /// Makes an Identifiable able to spawn.
    /// </summary>
    /// <param name="ident">The object's type</param>
    /// <param name="timeWindow">The time of day the object will spawn in.</param>
    /// <param name="zones">Which zones to spawn in.</param>
    /// <param name="weight">The chance to spawn.</param>
    /// <param name="spawnerTargets">Which spawner type to inject into.</param>
    /// <param name="mode">Which spawning behavior to use.</param>
    /// <param name="requiredConditions">Required conditions on when not to inject into a spawner.</param>
    /// <param name="customZoneSceneNames">An array of custom scene names.</param>
    /// <exception cref="InvalidOperationException">Occurs when an invalid SpawningMode is used. Should never happen.</exception>
    public static void MakeSpawnableInZones(IdentifiableType ident, DirectedActorSpawner.TimeWindow timeWindow,
        SpawnLocations zones, float weight, SpawnerTypes spawnerTargets, SpawningMode mode, 
        RequiredConditions requiredConditions, params string[] customZoneSceneNames)
    {
        
        var list = GetSceneNamesFromSpawnerZones(zones);

        foreach (var custom in customZoneSceneNames)
            list.Add(custom);
        
        switch (mode)
        {
            case SpawningMode.Default:

                executeOnSpawnerAwake.Add(new(spawner =>
                {
                    
                    if (ContainsZoneName(spawner.gameObject.scene.name, list))
                    {
                        if (spawnerTargets.HasFlag(SpawnerTypes.Slime))
                            if (spawner.TryCast<DirectedSlimeSpawner>() != null)
                                AddSpawningSettings(spawner, ident, weight, timeWindow, requiredConditions);

                        if (spawnerTargets.HasFlag(SpawnerTypes.Animal))
                            if (spawner.TryCast<DirectedAnimalSpawner>() != null)
                                AddSpawningSettings(spawner, ident, weight, timeWindow, requiredConditions);
                    }
                }));
                break;
            case SpawningMode.ReplacementBasedSpawning:
                spawnerReplacements.Add(new ReplacementSpawnerData()
                {
                    chance = weight,
                    ident = ident,
                    zones = list.ToArray(),
                });
                break;
            default:
                throw new InvalidOperationException("Unknown spawning mode used!");
        }

    }

    public static List<string> GetSceneNamesFromSpawnerZones(SpawnLocations zones)
    {
        List<string> names = new();

        if (zones.HasFlag(SpawnLocations.RainbowFields)) names.Add("zoneFields");
        if (zones.HasFlag(SpawnLocations.EmberValley)) names.Add("zoneGorge");
        if (zones.HasFlag(SpawnLocations.StarlightStand)) names.Add("zoneStrand");
        if (zones.HasFlag(SpawnLocations.PowderfallBluffs)) names.Add("zoneBluffs");
        if (zones.HasFlag(SpawnLocations.LabyrinthWaterworks)) names.Add("LabStrand");
        if (zones.HasFlag(SpawnLocations.LabyrinthLavadepths)) names.Add("LabValley");
        if (zones.HasFlag(SpawnLocations.LabyrinthDreamland)) names.Add("Dreamland");
        if (zones.HasFlag(SpawnLocations.LabyrinthHub)) names.Add("Hub");

        return names;
    }

    public static List<string> GetDefNamesFromSpawnerZones(SpawnLocations zones)
    {
        List<string> names = new();

        if (zones.HasFlag(SpawnLocations.RainbowFields)) names.Add("fields");
        if (zones.HasFlag(SpawnLocations.EmberValley)) names.Add("gorge");
        if (zones.HasFlag(SpawnLocations.StarlightStand)) names.Add("strand");
        if (zones.HasFlag(SpawnLocations.PowderfallBluffs)) names.Add("bluffs");
        if (zones.HasFlag(SpawnLocations.LabyrinthWaterworks)) names.Add("waterworks");
        if (zones.HasFlag(SpawnLocations.LabyrinthLavadepths)) names.Add("lavadepths");
        if (zones.HasFlag(SpawnLocations.LabyrinthDreamland)) names.Add("dream");
        if (zones.HasFlag(SpawnLocations.LabyrinthHub)) names.Add("hub");

        return names;
    }

    internal static bool ContainsZoneName(string sceneName, List<string> zoneNames)
    {
        foreach (var zoneName in zoneNames)
            if (sceneName.Contains(zoneName))
                return true;

        return false;
    }

    internal static bool IsInZone(string[] zoneNames)
    {
        if (sceneContext == null) return false;
        if (sceneContext.Player == null) return false;
        var tracker = sceneContext.Player.GetComponent<PlayerZoneTracker>();
        if (tracker == null) return false;
        var zone = tracker.GetCurrentZone();
        if (zone == null) return false;
        foreach (var name in zoneNames)
        {
            if (zone.name.ToLower().Contains(name.ToLower())) return true;
        }

        return false;
    }

    public static void AddSpawningSettings(DirectedActorSpawner spawner, IdentifiableType ident, float weight,
        DirectedActorSpawner.TimeWindow timeWindow, RequiredConditions requiredConditions)
    {
        
        DirectedActorSpawner.SpawnConstraint constraint = null;
        float consWeight = 0; // only used if a new constraint needs to be made.

        foreach (var cons in spawner.Constraints)
        {
            var allConditions = Enum.GetValues<RequiredConditions>();
            foreach (RequiredConditions con in allConditions)
            {
                if ((requiredConditions & con) == con)
                {
                    switch (con)
                    {
                        case RequiredConditions.RequiresAnglerSlime:
                            bool hasAngler = false;
                            foreach (var m in cons.Slimeset.Members)
                            {
                                if (m.IdentType.name.ToLower() == "angler")
                                {
                                    hasAngler = true;
                                    break;
                                }
                            }

                            if (!hasAngler)
                                return;
                            
                            break;
                        case RequiredConditions.RequiresAnglerLargo:
                            bool hasAnglerL = false;
                            foreach (var m in cons.Slimeset.Members)
                            {
                                if (m.IdentType.name.ToLower().Contains("angler") && m.IdentType.Cast<SlimeDefinition>().IsLargo)
                                {
                                    hasAnglerL = true;
                                    break;
                                }
                            }

                            if (!hasAnglerL)
                                return;

                            break;
                    
                        case RequiredConditions.RequiresLargosOnly:
                            bool hasNonLargo = false;
                            foreach (var m in cons.Slimeset.Members)
                            {
                                if (!m.IdentType.Cast<SlimeDefinition>().IsLargo)
                                {
                                    hasNonLargo = true;
                                    break;
                                }
                            }

                            if (hasNonLargo)
                                return;

                            break;
                    
                        case RequiredConditions.RequiresOnlyPinkOrModdedSlimes:
                            bool hasNonPink = false;
                            foreach (var m in cons.Slimeset.Members)
                            {
                                if (m.IdentType.name.ToLower() != "pink" && !savedIdents.ContainsValue(m.IdentType))
                                {
                                    hasNonPink = true;
                                    break;
                                }
                            }

                            if (hasNonPink)
                                return;

                            break;
                    }
                }
            }
            consWeight = ((consWeight / 2) + (cons.Weight / 2)) * 2;
            if (cons.Window.TimeMode == timeWindow.TimeMode)
            {
                constraint = cons;
                break;
            }
        }

        if (constraint == null)
        {
            constraint = new DirectedActorSpawner.SpawnConstraint();
            constraint.Window = timeWindow;
            constraint.Feral = false;
            constraint.Weight = consWeight;
            constraint.Slimeset = new SlimeSet();
            constraint.Slimeset.Members = new Il2CppReferenceArray<SlimeSet.Member>(0);
            spawner.Constraints = spawner.Constraints.Add(constraint);
        }

        var member = new SlimeSet.Member()
        {
            _prefab = ident.prefab,
            IdentType = ident,
            Weight = weight
        };
        constraint.Slimeset.Members = constraint.Slimeset.Members.Add(member);
    }

    internal struct ReplacementSpawnerData
    {
        public IdentifiableType ident;
        public float chance;
        public string[] zones;
    }

    public static void SetResourceGrower(
        IdentifiableType ident,
        float chance,
        int minAmount,
        string spawnerObjectName,
        SpawnLocations zones,
        params string[] blacklistInSpawnerName)
    {
        onResourceGrowerAwake.Add(spawner =>
        {
            var id = spawnerObjectName;
            var i = 0;
            foreach (var blacklist in blacklistInSpawnerName)
            {
                id += $"-blacklist{i}_{blacklist}";
                i++;
            }

            id += $"-zones_{(int)zones}";
            var scenes = GetSceneNamesFromSpawnerZones(zones);

            bool inZone = scenes.Any(scene => spawner.gameObject.scene.name.Contains(scene));

            if (!inZone) return;

            if (!spawner.gameObject.name.Contains(spawnerObjectName)) return;

            bool containsBlacklistedWord = blacklistInSpawnerName.Any(name => spawner.gameObject.name.Contains(name));

            if (containsBlacklistedWord) return;

            resourceGrowerDefinitions.TryGetValue(id, out var spawnerDefinition);

            if (spawnerDefinition == null)
            {
                spawnerDefinition = Object.Instantiate(spawner.ResourceGrowerDefinition);
                spawnerDefinition.name = id;
                Object.DontDestroyOnLoad(spawnerDefinition);
            }

            var _ = spawnerDefinition._resources.FirstOrDefault(x => x.ResourceIdentifiableType.name == ident.name);
            if (_ == null)
                spawnerDefinition._resources = spawnerDefinition._resources.Add(
                    new ResourceSpawnerDefinition.WeightedResourceEntry()
                    {
                        MinimumAmount = minAmount,
                        ResourceIdentifiableType = ident,
                        Weight = chance
                    });

            spawner._resourceGrowerDefinition = spawnerDefinition;

            resourceGrowerDefinitions.TryAdd(id, spawnerDefinition);
        });
    }

    public static void CreateGordoSpawnLocation(IdentifiableType ident, SpawnLocations zone, Vector3 position,
        Vector3 rotation, string cottonIdentifier)
    {
        var sceneName = GetSceneNamesFromSpawnerZones(zone)[0];

        if (!onSceneLoaded.TryGetValue(sceneName, out var actions))
        {
            actions = new List<Action>();
            onSceneLoaded.Add(sceneName, actions);
        }

        actions.Add(() =>
        {
            if (gordos.TryGetValue(cottonIdentifier, out var obj))
                if (obj == null)
                    gordos[cottonIdentifier] =
                        InstantiationHelpers.InstantiateDynamic(ident.prefab, position, Quaternion.Euler(rotation));
                else
                    return;
            else
                gordos.Add(cottonIdentifier,
                    InstantiationHelpers.InstantiateDynamic(ident.prefab, position, Quaternion.Euler(rotation)));
            
        });
    }

    public static Dictionary<string, GameObject> gordos = new Dictionary<string, GameObject>(); 
    
    public static Dictionary<string, List<Action>> onSceneLoaded = new Dictionary<string, List<Action>>();
    
    public static Dictionary<string, ResourceGrowerDefinition> resourceGrowerDefinitions = new();
    
    internal static List<Action<SpawnResource>> onResourceGrowerAwake = new ();
}