namespace SR2E.Prism.Data;

public class PrismLargo : PrismSlime
{

    internal PrismLargo(SlimeDefinition slimeDefinition, bool isNative): base(slimeDefinition, isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
    }
}