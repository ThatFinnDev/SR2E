namespace SR2E.Prism.Data;

public class PrismBaseSlime : PrismSlime
{
    internal bool _allowLargos = false;
    internal bool _disableAutoModdedLargos = false;
    public static implicit operator PrismBaseSlime(PrismNativeBaseSlime nativeBaseSlime)
    {
        return nativeBaseSlime.GetPrismBaseSlime();
    }

    public Material GetBaseMaterial()
    {
        try
        {
            foreach (var structure in GetSlimeAppearance()._structures)
                try
                {
                    if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                    {
                        return structure.DefaultMaterials[0];
                    }
                } catch {}
        } catch { }
        return null;
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