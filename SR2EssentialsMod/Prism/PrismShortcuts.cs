using SR2E.Prism.Data;

namespace SR2E.Prism;

public static class PrismShortcuts
{
    internal static Dictionary<SlimeDefinition, PrismSlime> _prismSlimes = new Dictionary<SlimeDefinition, PrismSlime>();
    internal static Dictionary<IdentifiableType, PrismPlort> _prismPlorts = new Dictionary<IdentifiableType, PrismPlort>();
    
    
    //Base Slime only
    public static PrismSlime GetPrismSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo) return null;
        if (_prismSlimes.ContainsKey(customOrNativeSlime)) return _prismSlimes[customOrNativeSlime];
        var newSlime = new PrismSlime(customOrNativeSlime, true);
        _prismSlimes.Add(customOrNativeSlime, newSlime);
        return newSlime;
    }
    
    
    //Base Slime only
    public static PrismPlort GetPrismPlort(this IdentifiableType customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsPlort) return null;
        if (_prismPlorts.ContainsKey(customOrNativeSlime)) return _prismPlorts[customOrNativeSlime];
        var newSlime = new PrismPlort(customOrNativeSlime, true);
        _prismPlorts.Add(customOrNativeSlime, newSlime);
        return newSlime;
    }
}