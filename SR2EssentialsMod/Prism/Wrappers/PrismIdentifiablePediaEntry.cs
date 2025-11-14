using Il2CppMonomiPark.SlimeRancher.Pedia;

namespace SR2E.Prism.Data;

public class PrismIdentifiablePediaEntry : PrismPediaEntry
{
    public static implicit operator IdentifiablePediaEntry(PrismIdentifiablePediaEntry identifiablePediaEntry)
    {
        return identifiablePediaEntry.GetIdentifiablePediaEntry();
    }
    public static implicit operator PrismIdentifiablePediaEntry(IdentifiablePediaEntry identifiablePediaEntry)
    {
        return identifiablePediaEntry.GetPrismIdentifiablePediaEntry();
    }
    public IdentifiablePediaEntry GetIdentifiablePediaEntry() => _pediaEntry.TryCast<IdentifiablePediaEntry>();
    public IdentifiableType GetIdentifiableType() => GetIdentifiablePediaEntry().IdentifiableType;
    internal PrismIdentifiablePediaEntry(PediaEntry pediaEntry, bool isNative): base(pediaEntry, isNative)
    {
        this._pediaEntry = pediaEntry;
        this._isNative = isNative;
    }
}