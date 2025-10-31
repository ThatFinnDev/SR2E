namespace SR2E.Prism.Data;

public class PrismBaseSlime : PrismSlime
{
    //public bool _canLargofy = false;
    //public bool _createAllLargos = false;
    internal PrismBaseSlime(SlimeDefinition slimeDefinition, bool isNative): base(slimeDefinition, isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
    }
    
}