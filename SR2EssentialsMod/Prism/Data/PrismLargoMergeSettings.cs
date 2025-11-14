namespace SR2E.Prism.Data;

public class PrismLargoMergeSettings
{
    public bool mergeComponents = true;
    public PrismBFMergeStrategy body;
    public PrismBFMergeStrategy face;
    public PrismColorMergeStrategy baseColors;
    public PrismColorMergeStrategy twinColors;
    public PrismColorMergeStrategy sloomberColors;

    public PrismLargoMergeSettings()
    {
        body = PrismBFMergeStrategy.Optimal;
        face = PrismBFMergeStrategy.Optimal;
        baseColors = PrismColorMergeStrategy.Optimal;
        twinColors = PrismColorMergeStrategy.Optimal;
        sloomberColors = PrismColorMergeStrategy.Optimal;
    }

    public PrismLargoMergeSettings(bool mergeComponents,PrismBFMergeStrategy body, PrismBFMergeStrategy face, PrismColorMergeStrategy baseColors, PrismColorMergeStrategy twinColors, PrismColorMergeStrategy sloomberColors)
    {
        this.mergeComponents=mergeComponents;
        this.body = body;
        this.face = face;
        this.baseColors = baseColors;
        this.twinColors = twinColors;
        this.sloomberColors = sloomberColors;
    }
}   