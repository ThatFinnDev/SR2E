using SR2E.Prism.Data;

namespace SR2E.Prism;

public static class PrismShortcuts
{
    internal static Dictionary<SlimeDefinition, PrismBaseSlime> _prismBaseSlimes = new Dictionary<SlimeDefinition, PrismBaseSlime>();
    internal static Dictionary<SlimeDefinition, PrismLargo> _prismLargos = new Dictionary<SlimeDefinition, PrismLargo>();
    internal static Dictionary<IdentifiableType, PrismPlort> _prismPlorts = new Dictionary<IdentifiableType, PrismPlort>();
    
    
    public static PrismBaseSlime GetPrismBaseSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo) return null;
        if (_prismBaseSlimes.ContainsKey(customOrNativeSlime)) return _prismBaseSlimes[customOrNativeSlime];
        var newSlime = new PrismBaseSlime(customOrNativeSlime, true);
        _prismBaseSlimes.Add(customOrNativeSlime, newSlime);
        return newSlime;
    }
    
    public static PrismLargo GetPrismLargo(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (!customOrNativeSlime.IsLargo) return null;
        if (_prismLargos.ContainsKey(customOrNativeSlime)) return _prismLargos[customOrNativeSlime];
        var newSlime = new PrismLargo(customOrNativeSlime, true);
        _prismLargos.Add(customOrNativeSlime, newSlime);
        return newSlime;
    }
    
    public static PrismSlime GetPrismSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo)
        {
            if (_prismLargos.ContainsKey(customOrNativeSlime)) return _prismBaseSlimes[customOrNativeSlime];
            var newLargo = new PrismLargo(customOrNativeSlime, true);
            _prismLargos.Add(customOrNativeSlime, newLargo);
            return newLargo;
        }
        if (_prismBaseSlimes.ContainsKey(customOrNativeSlime)) return _prismBaseSlimes[customOrNativeSlime];
        var newBaseSlime = new PrismBaseSlime(customOrNativeSlime, true);
        _prismBaseSlimes.Add(customOrNativeSlime, newBaseSlime);
        return newBaseSlime;
        
    }
    
    
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