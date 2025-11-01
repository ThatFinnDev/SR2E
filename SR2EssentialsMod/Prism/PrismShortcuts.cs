using Il2CppSystem.Linq;
using SR2E.Cotton;
using SR2E.Prism.Creators;
using SR2E.Prism.Data;
using SR2E.Prism.Enums;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism;

public static class PrismShortcuts
{
    internal static Dictionary<SlimeDefinition, PrismBaseSlime> _prismBaseSlimes = new Dictionary<SlimeDefinition, PrismBaseSlime>();
    internal static Dictionary<SlimeDefinition, PrismLargo> _prismLargos = new Dictionary<SlimeDefinition, PrismLargo>();

    internal static TripleDictionary<PrismLargo, PrismBaseSlime, PrismBaseSlime> _prismLargoBases = new TripleDictionary<PrismLargo, PrismBaseSlime, PrismBaseSlime>();
    internal static Dictionary<IdentifiableType, PrismPlort> _prismPlorts = new Dictionary<IdentifiableType, PrismPlort>();

    public static PrismBaseSlime GetPrismBaseSlime(this PrismNativeBaseSlime nativeBaseSlime)
    {
        var refID = nativeBaseSlime.GetReferenceID();
        foreach (var slime in CottonLibrary.baseSlimes._memberTypes)
            try {
                if (slime.ReferenceId == refID) return slime.TryCast<SlimeDefinition>().GetPrismBaseSlime();
            } catch { }
        return null;
    }
    public static PrismPlort GetPrismPlort(this PrismNativePlort nativePlort)
    {
        var refID = nativePlort.GetReferenceID();
        foreach (var plort in CottonLibrary.plorts._memberTypes)
            try
            {
                if (plort.ReferenceId == refID) return plort.GetPrismPlort();
            } catch { }
        return null;
    }
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

    
    public static PrismPlort GetPrismPlort(this IdentifiableType customOrNativePlort)
    {
        if (customOrNativePlort == null) return null;
        if (!customOrNativePlort.IsPlort) return null;
        if (_prismPlorts.ContainsKey(customOrNativePlort)) return _prismPlorts[customOrNativePlort];
        var newPlort = new PrismPlort(customOrNativePlort, true);
        _prismPlorts.Add(customOrNativePlort, newPlort);
        return newPlort;
    }
}