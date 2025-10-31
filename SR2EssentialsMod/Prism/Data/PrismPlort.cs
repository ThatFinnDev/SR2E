namespace SR2E.Prism.Data;

public class PrismPlort
{
    public static implicit operator IdentifiableType(PrismPlort prismPlort)
    {
        return prismPlort.GetIdentifiableType();
    }
    
    internal IdentifiableType _identifiableType;
    private bool _isNative;
    public bool GetIsNative() => _isNative;
    
    public IdentifiableType GetIdentifiableType() => _identifiableType;
    
    
    
    internal PrismPlort(IdentifiableType identifiableType, bool isNative)
    {
        this._identifiableType = identifiableType;
        this._isNative = isNative;
    }
}