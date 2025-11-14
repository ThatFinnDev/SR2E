using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Prism;

public static class Callbacks
{
    public delegate void OnPlortSold(int amount, IdentifiableType id);
    public delegate void OnZoneEnter(ZoneDefinition zone);
    public delegate void OnZoneExit(ZoneDefinition zone);


    public static event OnPlortSold onPlortSold;
    public static event OnZoneEnter onZoneEnter;
    public static event OnZoneExit onZoneExit;
    

    internal static void Invoke_onPlortSold(int amount, IdentifiableType id) => onPlortSold?.Invoke(amount, id);
    internal static void Invoke_onZoneEnter(ZoneDefinition zone) => onZoneEnter?.Invoke(zone);
    internal static void Invoke_onZoneExit(ZoneDefinition zone) => onZoneExit?.Invoke(zone);

}