using SR2E.Prism.Enums;

namespace SR2E.Prism.Data;

public class PrismBaseSlime : PrismSlime
{
    public static implicit operator PrismBaseSlime(PrismNativeBaseSlime nativeBaseSlime)
    {
        return nativeBaseSlime.GetPrismBaseSlime();
    }
    internal PrismBaseSlime(SlimeDefinition slimeDefinition, bool isNative): base(slimeDefinition, isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
    }
    
}