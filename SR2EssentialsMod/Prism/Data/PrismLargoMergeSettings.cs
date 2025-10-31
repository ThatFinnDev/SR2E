using SR2E.Prism.Enums;

namespace SR2E.Prism.Data;

public class PrismLargoMergeSettings
{
    public bool mergeComponents = true;
    public PrismTwoMergeStrategy body = PrismTwoMergeStrategy.KeepFirst;
    public PrismTwoMergeStrategy face = PrismTwoMergeStrategy.KeepFirst;
    public PrismThreeMergeStrategy baseColors = PrismThreeMergeStrategy.Merge;
    public PrismThreeMergeStrategy twinColors = PrismThreeMergeStrategy.Merge;
    public PrismThreeMergeStrategy sloomberColors = PrismThreeMergeStrategy.Merge;
    public PrismLargoMergeSettings() {}

    public PrismLargoMergeSettings(bool mergeComponents,PrismTwoMergeStrategy body, PrismTwoMergeStrategy face, PrismThreeMergeStrategy baseColors, PrismThreeMergeStrategy twinColors, PrismThreeMergeStrategy sloomberColors)
    {
        this.mergeComponents=mergeComponents;
        this.body = body;
        this.face = face;
        this.baseColors = baseColors;
        this.twinColors = twinColors;
        this.sloomberColors = sloomberColors;
    }
}   