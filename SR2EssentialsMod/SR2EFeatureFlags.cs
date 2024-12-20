namespace SR2E;

public static class SR2EFeatureFlags
{
    static SR2ECommand.CommandType enabledCMDs =
        SR2ECommand.CommandType.Cheat |
        SR2ECommand.CommandType.Binding |
        SR2ECommand.CommandType.Warp |
        SR2ECommand.CommandType.Common |
        SR2ECommand.CommandType.Menu |
        SR2ECommand.CommandType.Miscellaneous |
        SR2ECommand.CommandType.Fun;

    public static SR2ECommand.CommandType enabledCommands => enabledCMDs;
    static List<FeatureFlag> flags = new List<FeatureFlag>()
    {
        FeatureFlag.ExperimentalSettingsInjection,
        FeatureFlag.ExperimentalCommands, 
        FeatureFlag.DevMode,
    };

    static bool initialized = false;
    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        if (FeatureFlag.DevMode.hasFeature()) enabledCMDs |= SR2ECommand.CommandType.DevOnly;
        if (FeatureFlag.ExperimentalCommands.hasFeature()) enabledCMDs |= SR2ECommand.CommandType.Experimental;
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