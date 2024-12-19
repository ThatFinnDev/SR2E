namespace SR2E;

public static class SR2EFeatureFlags
{
    static List<FeatureFlag> flags = new List<FeatureFlag>()
    {
        FeatureFlag.ExperimentalSettingsInjection, FeatureFlag.ExperimentalCommands
    };
    public static List<FeatureFlag> FeatureFlags => flags;
    public static bool hasFeature(this FeatureFlag featureFlag) => flags.Contains(featureFlag);
    internal const int MAX_AUTOCOMPLETE = 55;
    internal const int MAX_CONSOLELINES = 150;
    internal const int SAVESLOT_COUNT = 75;
}

public enum FeatureFlag
{
    ExperimentalSettingsInjection, ExperimentalCommands
}