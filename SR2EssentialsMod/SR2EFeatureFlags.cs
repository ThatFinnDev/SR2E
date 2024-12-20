using System;

namespace SR2E;

public enum FeatureIntegerValues
{
    MAX_AUTOCOMPLETE,MAX_CONSOLELINES,SAVESLOT_COUNT, MAX_AUTOCOMPLETEONSCREEN
}
public static class SR2EFeatureFlags
{
    private static SR2ECommand.CommandType enabledCMDs;
    private static Dictionary<FeatureIntegerValues, int> featureObjects = new Dictionary<FeatureIntegerValues, int>()
    {
        {MAX_AUTOCOMPLETE,55},
        {MAX_CONSOLELINES,150},
        {SAVESLOT_COUNT,75},
        {MAX_AUTOCOMPLETEONSCREEN,6}
    };

    private static FeatureFlag enabledFlags =
        CommandsLoadCommands | CommandsLoadCheat | CommandsLoadBinding | CommandsLoadWarp | CommandsLoadCommon | CommandsLoadMenu | CommandsLoadMiscellaneous | CommandsLoadFun | 
        AllowCheats | AllowExpansions |
        EnableModMenu | EnableConsole | EnableIl2CppDetourExceptionReporting |
        InjectMainMenuButtons | InjectRanchUIButtons | InjectPauseButtons | InjectSR2Translations |
        AddCheatMenuButton | AddModMenuButton
        ;


    static bool initialized = false;
    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        if (CommandsLoadCommands.HasFlag())
        {
            if (DevMode.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.DevOnly;
            if (CommandsLoadExperimental.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Experimental;
            if(AllowCheats.HasFlag())
                if (CommandsLoadCheat.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Cheat;
            if (CommandsLoadBinding.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Binding;
            if (CommandsLoadWarp.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Warp;
            if (CommandsLoadCommon.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Common;
            if (CommandsLoadCommon.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Common;
            if (CommandsLoadMenu.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Menu;
            if (CommandsLoadMiscellaneous.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Miscellaneous;
            if (CommandsLoadFun.HasFlag()) enabledCMDs |= SR2ECommand.CommandType.Fun;
        }
        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {  
            switch (melonBase.Info.Name)
            {
                case "InfiniteEnergy":
                    enabledFlags |= DisableInfEnergy;
                    break;
                case "InfiniteHealth":
                    enabledFlags |= DisableInfHealth;
                    break;
                case "mSRML":
                    enabledFlags &= ~EnableConsole;
                    break;
            }
        }
    }
    
    public static SR2ECommand.CommandType enabledCommands => enabledCMDs;
    public static FeatureFlag flags => enabledFlags;
    public static bool HasFlag(this FeatureFlag featureFlag) => enabledFlags.HasFlag(featureFlag);

    public static int Get(this FeatureIntegerValues featureIntegerValues)
    {
        if (!featureObjects.ContainsKey(featureIntegerValues)) return 0;
        return featureObjects[featureIntegerValues];
    }
}

[Flags]
public enum FeatureFlag
{
    None = 0,
    DevMode = 1 << 1,
    CommandsLoadCommands = 1 << 2, //
    CommandsLoadExperimental = 1 << 3, //
    CommandsLoadCheat = 1 << 4, //
    CommandsLoadBinding = 1 << 5, //
    CommandsLoadWarp = 1 << 6, //
    CommandsLoadCommon = 1 << 7, //
    CommandsLoadMenu = 1 << 8, //
    CommandsLoadMiscellaneous = 1 << 9, //
    CommandsLoadFun = 1 << 10, //
    ExperimentalSettingsInjection = 1 << 11,
    DebugLogging = 1 << 12,
    ShowUnityErrors = 1 << 13,
    AllowCheats = 1 << 14, //
    EnableModMenu = 1 << 15, //
    EnableConsole = 1 << 16, //
    InjectMainMenuButtons = 1 << 17, //
    InjectRanchUIButtons = 1 << 18, //
    InjectPauseButtons = 1 << 19, //
    AddCheatMenuButton = 1 << 20, //
    AddModMenuButton = 1 << 21, //
    AllowExpansions = 1 << 22, //
    InjectSR2Translations = 1 << 23, //
    EnableIl2CppDetourExceptionReporting = 1 << 24, //
    DisableLocalizedVersionPatch = 1 << 25,
    DisableInfHealth = 1 << 26,
    DisableInfEnergy = 1 << 27,
}