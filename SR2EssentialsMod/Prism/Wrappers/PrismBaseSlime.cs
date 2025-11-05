namespace SR2E.Prism.Data;

public class PrismBaseSlime : PrismSlime
{
    internal bool _allowLargos = false;
    internal bool _disableAutoModdedLargos = false;
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
        if (!isNative)
            _disableAutoModdedLargos = true;
    }
    internal PrismBaseSlime(SlimeDefinition slimeDefinition, bool isNative, bool allowLargos, bool disableAutoModdedLargos): base(slimeDefinition, isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
        this._allowLargos = allowLargos;
        this._disableAutoModdedLargos = disableAutoModdedLargos;
    }
}