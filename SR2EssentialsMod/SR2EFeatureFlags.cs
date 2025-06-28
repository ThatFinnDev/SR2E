using System;
using System.IO;
using System.Linq;
using System.Xml;
using SR2E.Enums.Features;

namespace SR2E;




public static class SR2EFeatureFlags
{
    
    static List<FeatureFlag> defaultFlags = new List<FeatureFlag>() {
        CommandsLoadCommands,CommandsLoadCheat,CommandsLoadBinding,CommandsLoadWarp,
        CommandsLoadCommon,CommandsLoadMenu,CommandsLoadMiscellaneous,CommandsLoadFun, 
        AllowExpansions,EnableModMenu,EnableConsole,EnableIl2CppDetourExceptionReporting,
        InjectMainMenuButtons,InjectRanchUIButtons,InjectPauseButtons,InjectTranslations,
        AddCheatMenuButton,AddModMenuButton,CheckForUpdates,AllowAutoUpdate,EnableInfHealth,
        EnableInfEnergy,EnableCheatMenu,EnableLocalizedVersionPatch,EnableThemeMenu,EnableRepoMenu
        
    };

    private static FeatureFlag[] extraDevFlags => new[] {
        DevMode, Experiments, CommandsLoadDevOnly, CommandsLoadExperimental, IgnoreSaveErrors, ExperimentalSaveExport, ExperimentalKeyCodes
    };
    private static FeatureFlag[] extraBetaFlags => new []{None};
    private static FeatureFlag[] extraAlphaFlags => new []{None};
    
    private static bool[] flagsToForceOff = new bool[Enum.GetValues(typeof(FeatureFlag)).Length];
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
    private static bool[] enabledFlags = new bool[Enum.GetValues(typeof(FeatureFlag)).Length];
    
    static bool initialized = false;
    //internal static string flagfile_path = SR2EEntryPoint.instance.MelonAssembly.Assembly.Location+"/../../UserData/.sr2eflags.xml";

    internal static string flagfile_path => Application.persistentDataPath + "/"+SR2EEntryPoint.updateBranch+".sr2eflags.xml";
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
            
            if (_requirementsMap.ContainsKey(flag)) if(_requirementsMap[flag].Length!=0)
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

        if (true) //Save Version
        {
            XmlElement xmlElement = xmlDoc.CreateElement("LAST_SR2EVERSION");
            strings.AppendChild(xmlElement);
            xmlElement.SetAttribute("value",BuildInfo.DisplayVersion);
            xmlElement.SetAttribute("default", BuildInfo.DisplayVersion);
            xmlElement.SetAttribute("DO_NOT_EDIT", "please");
        }
        // Save the XML document to a file
        
        if (File.Exists(flagfile_path)) File.SetAttributes(flagfile_path, FileAttributes.Normal);
        xmlDoc.Save(flagfile_path);
        File.SetAttributes(flagfile_path, FileAttributes.Hidden);
    }

    static void LoadFromFlagFile()
    {
        flagsToForceOff = new bool[Enum.GetValues(typeof(FeatureFlag)).Length];;
        if (!File.Exists(flagfile_path)) { SaveToFlagFile(); return; }
        
        XmlDocument xmlDoc = new XmlDocument();
        try { xmlDoc.Load(flagfile_path); }
        catch {}

        XmlElement root = xmlDoc["SR2EFeatureFlags"];
        if (root == null) { SaveToFlagFile(); return; }
        
        XmlElement strings = root["FeatureStringValue"];
        if (strings != null)
        {
            if(strings["LAST_SR2EVERSION"]!=null)
            {
                if (strings["LAST_SR2EVERSION"].GetAttribute("value") != BuildInfo.DisplayVersion)
                { SaveToFlagFile(); return; }
            }
            foreach (XmlElement stringElement in strings.ChildNodes)
                if(stringElement.Name!="FeatureStringValue")
                    if (Enum.TryParse(stringElement.Name, out FeatureStringValue intValue))
                        featureStrings[intValue] = stringElement.GetAttribute("value");
        }

        XmlElement flags = root["FeatureFlags"];
        if (flags != null)
            foreach (XmlElement flagElement in flags.ChildNodes)
                if (Enum.TryParse(flagElement.Name, out FeatureFlag flag))
                    if (bool.TryParse(flagElement.GetAttribute("value"), out bool isEnabled))
                        flag.SetFlag(isEnabled);
        XmlElement ints = root["FeatureIntegerValues"];
        if (ints != null)
            foreach (XmlElement intElement in ints.ChildNodes)
                if (Enum.TryParse(intElement.Name, out FeatureIntegerValue intValue))
                    if (int.TryParse(intElement.GetAttribute("value"), out int intResult))
                        featureInts[intValue] = intResult;

        
        foreach (FeatureFlag flag in Enum.GetValues(typeof(FeatureFlag)))
        {
            if(flag.requirementsMet()) continue;
            flag.DisableFlag();
            flagsToForceOff[Convert.ToInt32(flag)]=true;
        }
        
        SaveToFlagFile();
    }

    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        enabledFlags = new bool[Enum.GetValues(typeof(FeatureFlag)).Length];
        flagsToForceOff = new bool[Enum.GetValues(typeof(FeatureFlag)).Length];
        FeatureFlag[] addedFlags = null;
        switch (SR2EEntryPoint.updateBranch)
        {
            case "dev": addedFlags = extraDevFlags; break;
            case "alpha": addedFlags = extraAlphaFlags; break;
            case "beta": addedFlags= extraBetaFlags; break;
        }
        if(addedFlags!=null)
            foreach (FeatureFlag flag in addedFlags)
                defaultFlags.Add(flag);
        foreach (FeatureFlag flag in defaultFlags)
            flag.EnableFlag();
        featureInts = new Dictionary<FeatureIntegerValue, int>(defaultFeatureInts);
        featureStrings = new Dictionary<FeatureStringValue, string>(defaultFeatureStrings);
        try
        {
            if (File.Exists(flagfile_path)) LoadFromFlagFile();
            else SaveToFlagFile();
        }
        catch { }
        

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
    public static bool[] flags => enabledFlags;
    public static bool HasFlag(this FeatureFlag featureFlag) => enabledFlags[Convert.ToInt32(featureFlag)];
    public static bool HasFlag(this bool[] array,FeatureFlag featureFlag) => array[Convert.ToInt32(featureFlag)];
    public static bool HasFlag(this FeatureFlag[] array,FeatureFlag featureFlag) => array.Contains(featureFlag);
    public static bool HasFlag(this List<FeatureFlag> list,FeatureFlag featureFlag) => list.Contains(featureFlag);
    static void SetFlag(this FeatureFlag featureFlag, bool state) => enabledFlags[Convert.ToInt32(featureFlag)]=state;
    static bool EnableFlag(this FeatureFlag featureFlag) => enabledFlags[Convert.ToInt32(featureFlag)]=true;
    static bool DisableFlag(this FeatureFlag featureFlag) => enabledFlags[Convert.ToInt32(featureFlag)]=false;

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
        {CheckForUpdates,new FFR[]{new FFRDeactivated(DevMode)}},
        {AllowAutoUpdate,new FFR[]{new FFRDeactivated(DevMode)}},
        {EnableConsole,new FFR[]{new FFRMelonUnInstalled("mSRML")}},
        {EnableInfHealth,new FFR[]{new FFRMelonUnInstalled("InfiniteHealth")}},
        {EnableInfEnergy,new FFR[]{new FFRMelonUnInstalled("InfiniteEnergy")}},
        {CommandsLoadExperimental,new FFR[]{new FFRActivated(CommandsLoadCommands), new FFRActivated(Experiments)}},
        {CommandsLoadDevOnly,new FFR[]{new FFRActivated(CommandsLoadCommands), new FFRActivated(DevMode)}},
        {CommandsLoadCheat,new FFR[]{new FFRActivated(CommandsLoadCommands),new FFRDeactivated(DisableCheats)}},
        {CommandsLoadBinding,new FFR[]{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadWarp,new FFR[]{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadCommon,new FFR[]{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadMenu,new FFR[]{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadMiscellaneous,new FFR[]{new FFRActivated(CommandsLoadCommands)}},
        {CommandsLoadFun,new FFR[]{new FFRActivated(CommandsLoadCommands)}},
        {EnableCheatMenu, new FFR[]{new FFRDeactivated(DisableCheats)}},
        {AddCheatMenuButton,new FFR[]{new FFRActivated(EnableCheatMenu), new FFRActivated(InjectPauseButtons)}},
        {AddModMenuButton,new FFR[]{new FFRActivated(InjectMainMenuButtons), new FFRActivated(InjectPauseButtons)}},
        {ExperimentalSaveExport,new FFR[]{new FFRActivated(Experiments)}},
        {ExperimentalSettingsInjection,new FFR[]{new FFRActivated(Experiments)}},
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