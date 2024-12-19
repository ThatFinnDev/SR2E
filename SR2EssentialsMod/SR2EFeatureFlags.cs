namespace SR2E;

public static class SR2EFeatureFlags
{
    static List<FeatureFlag> flags = new List<FeatureFlag>()
    {
        FeatureFlag.ExperimentalSettingsInjection
    };
    public static List<FeatureFlag> FeatureFlags => flags;
    public static bool hasFeature(this FeatureFlag featureFlag) => flags.Contains(featureFlag);
}

public enum FeatureFlag
{
    ExperimentalSettingsInjection,
}