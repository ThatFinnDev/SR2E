using CottonLibrary.Save;
using Il2CppMonomiPark.SlimeRancher.World;
using Il2Cpp;
namespace CottonLibrary;

public static class Callbacks
{
    public delegate void OnPlortSold(int amount, IdentifiableType id);
    public delegate void OnZoneEnter(ZoneDefinition zone);
    public delegate void OnZoneExit(ZoneDefinition zone);
    public delegate void OnModdedSave(ModdedV01 save);


    public static event OnPlortSold onPlortSold;
    public static event OnZoneEnter onZoneEnter;
    public static event OnZoneExit onZoneExit;
    
    /// <summary>
    /// Used for when modded save data gets saved.
    /// </summary>
    public static event OnModdedSave onModdedSave;
    
    /// <summary>
    /// Used for when modded save data gets loaded.
    /// </summary>
    public static event OnModdedSave onModdedLoad;

    internal static void Invoke_onPlortSold(int amount, IdentifiableType id) => onPlortSold?.Invoke(amount, id);
    internal static void Invoke_onZoneEnter(ZoneDefinition zone) => onZoneEnter?.Invoke(zone);
    internal static void Invoke_onZoneExit(ZoneDefinition zone) => onZoneExit?.Invoke(zone);
    internal static void Invoke_onModdedSave(ModdedV01 save) => onModdedSave?.Invoke(save);
    internal static void Invoke_onModdedLoad(ModdedV01 save) => onModdedLoad?.Invoke(save);

}