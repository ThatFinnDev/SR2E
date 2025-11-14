namespace SR2E.Prism.Data;

public enum PrismSpawnConditions
{
    
    None = 0,

    /// <summary>
    /// Requires an Angler Slime to meet the condition.
    /// </summary>
    RequiresAnglerSlime = 10,

    /// <summary>
    /// Requires any Angler Largo to meet the condition.
    /// </summary>
    RequiresAnglerLargo = 20,

    /// <summary>
    /// Requires only Largo Slimes to meet the condition, if there is a normal slime that can be spawned, it will NOT meet the condition.
    /// </summary>
    RequiresLargosOnly = 30,

    /// <summary>
    /// Requires only Pink Slimes or any Modded Slime to meet the condition. If there are any other slimes, including largos, the condition will NOT be met.
    /// </summary>
    RequiresOnlyPinkOrModdedSlimes = 40,
}