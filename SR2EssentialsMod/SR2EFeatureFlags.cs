using System;
using System.IO;
using System.Xml;

namespace SR2E;

public enum FeatureIntegerValue
{
    MAX_AUTOCOMPLETE,MAX_CONSOLELINES,SAVESLOT_COUNT, MAX_AUTOCOMPLETEONSCREEN
}
public enum FeatureStringValue
{
    TEST_STRING
}
public static class SR2EFeatureFlags
{
    
    const FeatureFlag defaultFlags =
            CommandsLoadCommands | CommandsLoadCheat | CommandsLoadBinding | CommandsLoadWarp | CommandsLoadCommon | CommandsLoadMenu | CommandsLoadMiscellaneous | CommandsLoadFun | 
            AllowCheats | AllowExpansions |
            EnableModMenu | EnableConsole | EnableIl2CppDetourExceptionReporting |
            InjectMainMenuButtons | InjectRanchUIButtons | InjectPauseButtons | InjectSR2Translations |
            AddCheatMenuButton | AddModMenuButton |
            CheckForUpdates | AllowAutoUpdate;
    private static Dictionary<FeatureIntegerValue, int> defaultFeatureInts = new Dictionary<FeatureIntegerValue, int>()
    {
        {MAX_AUTOCOMPLETE,55},
        {MAX_CONSOLELINES,150},
        {SAVESLOT_COUNT,75},
        {MAX_AUTOCOMPLETEONSCREEN,6}
    };
    private static Dictionary<FeatureStringValue, string> defaultFeatureStrings = new Dictionary<FeatureStringValue, string>()
    {
        {TEST_STRING,"test"}
    };
    
    
    private static SR2ECommand.CommandType enabledCMDs;
    private static Dictionary<FeatureIntegerValue, int> featureints = new Dictionary<FeatureIntegerValue, int>();
    private static Dictionary<FeatureStringValue, string> featurestrings = new Dictionary<FeatureStringValue, string>();
    private static FeatureFlag enabledFlags = None;
    
    static bool initialized = false;
    internal static string flagfile_path = SR2EEntryPoint.instance.MelonAssembly.Assembly.Location+"/../../UserData/.sr2eflags.xml";

    static void SaveToFlagFile()
    {
        XmlDocument xmlDoc = new XmlDocument();
        
        XmlElement root = xmlDoc.CreateElement("SR2EFeatureFlags");
        xmlDoc.AppendChild(root);
        XmlElement flags = xmlDoc.CreateElement("FeatureFlags");
        root.AppendChild(flags);
        XmlElement ints = xmlDoc.CreateElement("FeatureIntegerValues");
        root.AppendChild(ints);
        XmlElement strings = xmlDoc.CreateElement("FeatureStringValue");
        root.AppendChild(strings);
        foreach (FeatureFlag flag in Enum.GetValues(typeof(FeatureFlag)))
        {
            //bools
            if (flag == None) continue;
            XmlElement xmlElement = xmlDoc.CreateElement(flag.ToString());
            flags.AppendChild(xmlElement);
            xmlElement.SetAttribute("value",flag.HasFlag().ToString().ToLower());
            xmlElement.SetAttribute("default", defaultFlags.HasFlag(flag).ToString().ToLower());
        }
        foreach (FeatureIntegerValue value in Enum.GetValues(typeof(FeatureIntegerValue)))
        {
            //ints
            XmlElement xmlElement = xmlDoc.CreateElement(value.ToString());
            ints.AppendChild(xmlElement);
            xmlElement.SetAttribute("value",value.Get().ToString().ToLower());
            xmlElement.SetAttribute("default", value.GetDefault().ToString().ToLower());
        }
        foreach (FeatureStringValue value in Enum.GetValues(typeof(FeatureStringValue)))
        {
            //strings
            XmlElement xmlElement = xmlDoc.CreateElement(value.ToString());
            strings.AppendChild(xmlElement);
            xmlElement.SetAttribute("value",value.Get().ToLower());
            xmlElement.SetAttribute("default", value.GetDefault().ToLower());
        }
        // Save the XML document to a file
        xmlDoc.Save(flagfile_path);
        File.SetAttributes(flagfile_path, FileAttributes.Hidden);
    }

    static void LoadFromFlagFile()
    {
        if (!File.Exists(flagfile_path)) { SaveToFlagFile(); return; }
        
        XmlDocument xmlDoc = new XmlDocument();
        try { xmlDoc.Load(flagfile_path); }
        catch {}

        XmlElement root = xmlDoc["SR2EFeatureFlags"];
        if (root == null) { SaveToFlagFile(); return; }

        XmlElement flags = root["FeatureFlags"];
        if (flags != null)
            foreach (XmlElement flagElement in flags.ChildNodes)
                if (Enum.TryParse(flagElement.Name, out FeatureFlag flag))
                    if (bool.TryParse(flagElement.GetAttribute("value"), out bool isEnabled))
                        if(isEnabled) enabledFlags |= flag;
                        else enabledFlags &= ~flag;
        XmlElement ints = root["FeatureIntegerValues"];
        if (ints != null)
            foreach (XmlElement intElement in ints.ChildNodes)
                if (Enum.TryParse(intElement.Name, out FeatureIntegerValue intValue))
                    if (int.TryParse(intElement.GetAttribute("value"), out int intResult))
                        featureints[intValue] = intResult;
        XmlElement strings = root["FeatureStringValue"];
        if (strings != null)
            foreach (XmlElement stringElement in strings.ChildNodes)
                if (Enum.TryParse(stringElement.Name, out FeatureStringValue intValue))
                    featurestrings[intValue] = stringElement.GetAttribute("value");

        SaveToFlagFile();
    }

    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        enabledFlags = defaultFlags;
        featureints = new Dictionary<FeatureIntegerValue, int>(defaultFeatureInts);
        featurestrings = new Dictionary<FeatureStringValue, string>(defaultFeatureStrings);
        if (File.Exists(flagfile_path)) LoadFromFlagFile();
        else SaveToFlagFile();
        
        if (DevMode.HasFlag())
        {
            enabledFlags &= ~CheckForUpdates;
            enabledFlags &= ~AllowAutoUpdate;
        }
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

    public static int Get(this FeatureIntegerValue featureIntegerValue)
    {
        if (!featureints.ContainsKey(featureIntegerValue)) return 0;
        return featureints[featureIntegerValue];
    }
    public static string Get(this FeatureStringValue featureStringValue)
    {
        if (!featurestrings.ContainsKey(featureStringValue)) return "";
        return featurestrings[featureStringValue];
    }
    public static int GetDefault(this FeatureIntegerValue featureIntegerValue)
    {
        if (!defaultFeatureInts.ContainsKey(featureIntegerValue)) return 0;
        return defaultFeatureInts[featureIntegerValue];
    }
    public static string GetDefault(this FeatureStringValue featureStringValue)
    {
        if (!defaultFeatureStrings.ContainsKey(featureStringValue)) return "";
        return defaultFeatureStrings[featureStringValue];
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
    CheckForUpdates = 1 << 28, //
    AllowAutoUpdate = 1 << 29, //
}