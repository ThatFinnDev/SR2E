using Il2CppMonomiPark.SlimeRancher.Pedia;

namespace SR2E.Prism.Data;

public class PrismFixedPediaEntry : PrismPediaEntry
{
    public static implicit operator FixedPediaEntry(PrismFixedPediaEntry fixedPediaEntry)
    {
        return fixedPediaEntry.GetFixedPediaEntry();
    }
    public static implicit operator PrismFixedPediaEntry(FixedPediaEntry fixedPediaEntry)
    {
        return fixedPediaEntry.GetPrismFixedPediaEntry();
    }
    public FixedPediaEntry GetFixedPediaEntry() => _pediaEntry.TryCast<FixedPediaEntry>();
    
    internal PrismFixedPediaEntry(PediaEntry pediaEntry, bool isNative): base(pediaEntry, isNative)
    {
        this._pediaEntry = pediaEntry;
        this._isNative = isNative;
    }
    
    public void SetIcon(Sprite newIcon)
    {
        GetFixedPediaEntry()._icon = newIcon;
    }
}