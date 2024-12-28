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
    DEFAULT_LANGUAGECODE
}
public static class SR2EFeatureFlags
{
    
    const FeatureFlag defaultFlags =
            CommandsLoadCommands | CommandsLoadCheat | CommandsLoadBinding | CommandsLoadWarp | CommandsLoadCommon | CommandsLoadMenu | CommandsLoadMiscellaneous | CommandsLoadFun | 
            AllowExpansions |
            EnableModMenu | EnableConsole | EnableIl2CppDetourExceptionReporting |
            InjectMainMenuButtons | InjectRanchUIButtons | InjectPauseButtons | InjectSR2Translations | CustomSettingsInjection |
            AddCheatMenuButton | AddModMenuButton |
            CheckForUpdates | AllowAutoUpdate | 
            EnableInfHealth | EnableInfEnergy | EnableCheatMenu | EnableLocalizedVersionPatch;
    static FeatureFlag flagsToForceOff;
    private static Dictionary<FeatureIntegerValue, int> defaultFeatureInts = new Dictionary<FeatureIntegerValue, int>()
    {
        {MAX_AUTOCOMPLETE,55},
        {MAX_CONSOLELINES,150},
        {SAVESLOT_COUNT,75},
        {MAX_AUTOCOMPLETEONSCREEN,6}
    };
    private static Dictionary<FeatureStringValue, string> defaultFeatureStrings = new Dictionary<FeatureStringValue, string>()
    {
        {DEFAULT_LANGUAGECODE,"en"}
    };
    
    
    private static CommandType enabledCMDs;
    private static Dictionary<FeatureIntegerValue, int> featureInts = new Dictionary<FeatureIntegerValue, int>();
    private static Dictionary<FeatureStringValue, string> featureStrings = new Dictionary<FeatureStringValue, string>();
    private static FeatureFlag enabledFlags = None;
    
    static bool initialized = false;
    //internal static string flagfile_path = SR2EEntryPoint.instance.MelonAssembly.Assembly.Location+"/../../UserData/.sr2eflags.xml";

    internal static string flagfile_path => Application.persistentDataPath + "/.sr2eflags.xml";
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
            if(!flagsToForceOff.HasFlag(flag)) xmlElement.SetAttribute("value",flag.HasFlag().ToString().ToLower());
            else xmlElement.SetAttribute("value","false");
            xmlElement.SetAttribute("default", defaultFlags.HasFlag(flag).ToString().ToLower());
            
            if (_requirementsMap.ContainsKey(flag)) if(_requirementsMap[flag]!=null) if(_requirementsMap[flag].Length!=0)
                foreach (FFR req in _requirementsMap[flag])
                {
                    if (req == null) continue;
                    string name = req.GetType().Name;
                    if (req is FFRString ffrString)
                    {
                        string exist = xmlElement.GetAttribute(name);
                        if (!String.IsNullOrEmpty(exist)) exist += ",";
                        xmlElement.SetAttribute(name, exist+ffrString.String);
                    }
                    else if (req is FFRFlag ffrFlag)
                    {
                        string exist = xmlElement.GetAttribute(name);
                        if (!String.IsNullOrEmpty(exist)) exist += ",";
                        xmlElement.SetAttribute(name, exist+ffrFlag.Flag);
                    }
                }
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
        
        if (File.Exists(flagfile_path)) File.SetAttributes(flagfile_path, FileAttributes.Normal);
        xmlDoc.Save(flagfile_path);
        File.SetAttributes(flagfile_path, FileAttributes.Hidden);
    }

    static void LoadFromFlagFile()
    {
        flagsToForceOff = 0;
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
                        featureInts[intValue] = intResult;
        XmlElement strings = root["FeatureStringValue"];
        if (strings != null)
            foreach (XmlElement stringElement in strings.ChildNodes)
                if (Enum.TryParse(stringElement.Name, out FeatureStringValue intValue))
                    featureStrings[intValue] = stringElement.GetAttribute("value");

        
        foreach (FeatureFlag flag in Enum.GetValues(typeof(FeatureFlag)))
        {
            if(flag.requirementsMet()) continue;
            enabledFlags &= ~flag;
            flagsToForceOff |= flag;
        }
        
        SaveToFlagFile();
    }

    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        enabledFlags = defaultFlags;
        featureInts = new Dictionary<FeatureIntegerValue, int>(defaultFeatureInts);
        featureStrings = new Dictionary<FeatureStringValue, string>(defaultFeatureStrings);
        if (File.Exists(flagfile_path)) LoadFromFlagFile();
        else SaveToFlagFile();

        
        if (CommandsLoadDevOnly.HasFlag()) enabledCMDs |= CommandType.DevOnly;
        if (CommandsLoadExperimental.HasFlag()) enabledCMDs |= CommandType.Experimental;
        if (CommandsLoadCheat.HasFlag()) enabledCMDs |= CommandType.Cheat;
        if (CommandsLoadBinding.HasFlag()) enabledCMDs |= CommandType.Binding;
        if (CommandsLoadWarp.HasFlag()) enabledCMDs |= CommandType.Warp;
        if (CommandsLoadCommon.HasFlag()) enabledCMDs |= CommandType.Common;
        if (CommandsLoadCommon.HasFlag()) enabledCMDs |= CommandType.Common;
        if (CommandsLoadMenu.HasFlag()) enabledCMDs |= CommandType.Menu;
        if (CommandsLoadMiscellaneous.HasFlag()) enabledCMDs |= CommandType.Miscellaneous;
        if(CommandsLoadFun.HasFlag()) enabledCMDs |= CommandType.Fun;
    }
    
    public static CommandType enabledCommands => enabledCMDs;
    public static FeatureFlag flags => enabledFlags;
    public static bool HasFlag(this FeatureFlag featureFlag) => enabledFlags.HasFlag(featureFlag);

    public static int Get(this FeatureIntegerValue featureIntegerValue)
    {
        if (!featureInts.ContainsKey(featureIntegerValue)) return 0;
        return featureInts[featureIntegerValue];
    }
    public static string Get(this FeatureStringValue featureStringValue)
    {
        if (!featureStrings.ContainsKey(featureStringValue)) return "";
        return featureStrings[featureStringValue];
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
    static Dictionary<FeatureFlag,FFR[]> _requirementsMap = new Dictionary<FeatureFlag, FFR[]>()
    {
        {CheckForUpdates,new []{new FFRDeactivated(DevMode)}},
        {AllowAutoUpdate,new []{new FFRDeactivated(DevMode)}},
        {EnableConsole,new []{new FFRMelonUnInstalled("mSRML")}},
        {EnableInfHealth,new []{new FFRMelonUnInstalled("InfiniteHealth")}},
        {EnableInfEnergy,new []{new FFRMelonUnInstalled("InfiniteEnergy")}},
        {CommandsLoadExperimental,new []{new FFRActivated(CommandsLoadCommands), new FFRActivated(Experiments)}},
        {CommandsLoadDevOnly,new []{new FFRActivated(CommandsLoadCommands), new FFRActivated(DevMode)}},
        {CommandsLoadCheat,new []{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadBinding,new []{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadWarp,new []{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadCommon,new []{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadMenu,new []{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadMiscellaneous,new []{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadFun,new []{new FFRActivated(CommandsLoadCommands)}},
        {AddCheatMenuButton,new []{new FFRActivated(EnableCheatMenu), new FFRActivated(InjectPauseButtons)}},
        {AddModMenuButton,new []{new FFRActivated(InjectMainMenuButtons), new FFRActivated(InjectPauseButtons)}},

    };
    static bool requirementsMet(this FeatureFlag featureFlag)
    {
        if (!_requirementsMap.ContainsKey(featureFlag)) return true;
        if(_requirementsMap[featureFlag]==null) return true;
        if(_requirementsMap[featureFlag].Length==0) return true;
        foreach (FFR req in _requirementsMap[featureFlag])
        {
            if(req==null) continue;
            if (req is FFRActivated activated)
            {
                if (!activated.Flag.HasFlag()) return false;
            }
            else if (req is FFRDeactivated deactivated)
            {
                if (deactivated.Flag.HasFlag()) return false;
            }
            else if (req is FFRMelonInstalled melonInstalled)
            {
                bool installed = false;
                foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
                {
                    if (melonBase.Info.Name == melonInstalled.String) { installed=true; break; }
                }
                if(!installed) return false;
            }
            else if (req is FFRMelonUnInstalled melonUninstalled)
            {
                foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
                    if(melonBase.Info.Name==melonUninstalled.String) return false;
            }
        }
        return true;
    }
}

[Flags]
public enum FeatureFlag : long
{
    None = 0,
    //Dev
    DevMode = 1L << 1,
    DebugLogging = 1L << 2,
    ShowUnityErrors = 1L << 3,
    Experiments = 1L << 4,
    CustomSettingsInjection = 1L << 5,
    
    //Commands+Dev
    CommandsLoadDevOnly = 1L << 6, 
    CommandsLoadExperimental = 1L << 7, 
    
    //Commands
    CommandsLoadCommands = 1L << 8, //
    CommandsLoadCheat = 1L << 9, //
    CommandsLoadBinding = 1L << 10, //
    CommandsLoadWarp = 1L << 11, //
    CommandsLoadCommon = 1L << 12, //
    CommandsLoadMenu = 1L << 13, //
    CommandsLoadMiscellaneous = 1L << 14, //
    CommandsLoadFun = 1L << 15, //

    //Cheats and Mods
    AddCheatMenuButton = 1L << 16, //
    EnableInfHealth = 1L << 17, //
    EnableInfEnergy = 1L << 18, //
    
    //Misc
    AddModMenuButton = 1L << 19, //
    AllowExpansions = 1L << 20, //
    EnableLocalizedVersionPatch = 1L << 21, //
    InjectSR2Translations = 1L << 22, //
    EnableIl2CppDetourExceptionReporting = 1L << 23, //
    
    //Menus
    EnableModMenu = 1L << 24, //
    EnableCheatMenu = 1L << 25, //
    EnableConsole = 1L << 26, //
    
    //UI
    InjectMainMenuButtons = 1L << 27, //
    InjectRanchUIButtons = 1L << 28, //
    InjectPauseButtons = 1L << 29, //

    //Updates and Patches
    CheckForUpdates = 1L << 30, //
    AllowAutoUpdate = 1L << 31, //

}
internal class FFR //FeatureFlagRequirement
{ } 
internal class FFRString : FFR //FeatureFlagRequirementString
{ 
    public string String;
} 
internal class FFRFlag : FFR  //FeatureFlagRequirementFlag
{ 
    public FeatureFlag Flag;
} 
internal class FFRDeactivated : FFRFlag //FeatureFlagRequirementDeactivated
{
    public FFRDeactivated(FeatureFlag Flag)
    { this.Flag = Flag; }
}
internal class FFRActivated : FFRFlag //FeatureFlagRequirementActivated
{
    public FFRActivated(FeatureFlag Flag)
    { this.Flag = Flag; }
}
internal class FFRMelonInstalled : FFRString //FeatureFlagRequirementMelonInstalled
{
    public FFRMelonInstalled(string MelonName)
    { this.String = MelonName; }
}
internal class FFRMelonUnInstalled : FFRString //FeatureFlagRequirementMelonInstalled
{
    public FFRMelonUnInstalled(string MelonName)
    { this.String = MelonName; }
}