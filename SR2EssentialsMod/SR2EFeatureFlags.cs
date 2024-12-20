namespace SR2E;

public static class SR2EFeatureFlags
{
    static SR2Command.CommandType enabledCommands =
        SR2Command.CommandType.Cheat |
        SR2Command.CommandType.Binding |
        SR2Command.CommandType.Warp |
        SR2Command.CommandType.Common |
        SR2Command.CommandType.Menu |
        SR2Command.CommandType.Miscellaneous |
        SR2Command.CommandType.Fun;
    static List<FeatureFlag> flags = new List<FeatureFlag>()
    {
        FeatureFlag.ExperimentalSettingsInjection, FeatureFlag.ExperimentalCommands, FeatureFlag.DevMode,
    };

    static bool initialized = false;
    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        if (FeatureFlag.DevMode.hasFeature())
            enabledCommands = SR2Command.CommandType.DevOnly;
    }
    public static List<FeatureFlag> FeatureFlags => flags;
    public static bool hasFeature(this FeatureFlag featureFlag) => flags.Contains(featureFlag);
    internal const int MAX_AUTOCOMPLETE = 55;
    internal const int MAX_CONSOLELINES = 150;
    internal const int SAVESLOT_COUNT = 75;
}

public enum FeatureFlag
{
    DevMode, ExperimentalSettingsInjection, ExperimentalCommands
}