using System;

namespace SR2E;

public static class SR2EFeatureFlags
{
    private static SR2ECommand.CommandType enabledCMDs;
    public static SR2ECommand.CommandType enabledCommands => enabledCMDs;

    private static FeatureFlag enabledFlags =
        CommandsLoadCommands |
        CommandsLoadCheat |
        CommandsLoadBinding |
        CommandsLoadWarp |
        CommandsLoadCommon |
        CommandsLoadMenu |
        CommandsLoadMiscellaneous |
        CommandsLoadFun;

    static bool initialized = false;
    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        if (CommandsLoadCommands.HasFlag())
        {
            if (DevMode.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.DevOnly;
            if (CommandsLoadExperimental.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Experimental;
            if (CommandsLoadCheat.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Cheat;
            if (CommandsLoadBinding.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Binding;
            if (CommandsLoadWarp.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Warp;
            if (CommandsLoadCommon.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Common;
            if (CommandsLoadCommon.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Common;
            if (CommandsLoadMenu.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Menu;
            if (CommandsLoadMiscellaneous.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Miscellaneous;
            if (CommandsLoadFun.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Fun;
        }
    }
    public static FeatureFlag flags => enabledFlags;
    public static bool HasFlag(this FeatureFlag featureFlag) => enabledFlags.HasFlag(featureFlag);
    internal const int MAX_AUTOCOMPLETE = 55;
    internal const int MAX_CONSOLELINES = 150;
    internal const int SAVESLOT_COUNT = 75;
}

[Flags]
public enum FeatureFlag
{
    None = 0,
    DevMode = 1 << 1,
    CommandsLoadCommands = 1 << 2,
    CommandsLoadExperimental = 1 << 3,
    CommandsLoadCheat = 1 << 4,
    CommandsLoadBinding = 1 << 5,
    CommandsLoadWarp = 1 << 6,
    CommandsLoadCommon = 1 << 7,
    CommandsLoadMenu = 1 << 8,
    CommandsLoadMiscellaneous = 1 << 9,
    CommandsLoadFun = 1 << 10,
    ExperimentalSettingsInjection = 1 << 11,
}