namespace SR2E.Prism.Data;

public struct PrismMarketData
{
    public readonly float saturation;
    public readonly float value;
    public readonly bool hideInMarketUI;

    public PrismMarketData(float saturation, float value)
    {
        this.saturation = saturation;
        this.value = value;
        this.hideInMarketUI = false;
    }
    public PrismMarketData(float saturation, float value, bool hideInMarketUI)
    {
        this.saturation = saturation;
        this.value = value;
        this.hideInMarketUI = hideInMarketUI;
    }
}