namespace CottonLibrary;

internal struct ModdedMarketData
{
    public readonly float saturation;
    public readonly float value;

    internal ModdedMarketData(float sat, float val)
    {
        value = val;
        saturation = sat;
    }
}