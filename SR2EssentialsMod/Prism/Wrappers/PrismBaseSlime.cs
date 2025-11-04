namespace SR2E.Prism.Data;

public class PrismBaseSlime : PrismSlime
{
    internal bool _allowLargos = false;
    public static implicit operator PrismBaseSlime(PrismNativeBaseSlime nativeBaseSlime)
    {
        return nativeBaseSlime.GetPrismBaseSlime();
    }
    internal PrismBaseSlime(SlimeDefinition slimeDefinition, bool isNative): base(slimeDefinition, isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
        if (_slimeDefinition.CanLargofy)
            _allowLargos = true;
    }
    internal PrismBaseSlime(SlimeDefinition slimeDefinition, bool isNative, bool allowLargos): base(slimeDefinition, isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
        this._allowLargos = allowLargos;
    }
}