using System;
using System.IO;
using System.Linq;
using System.Xml;
using SR2E.Enums.Features;
using SR2E.Managers;

namespace SR2E;




public static class SR2EFeatureFlags
{
    
    static List<FeatureFlag> defaultFlags = new List<FeatureFlag>() {
        CommandsLoadCommands,CommandsLoadCheat,CommandsLoadBinding,CommandsLoadWarp,
        CommandsLoadCommon,CommandsLoadMenu,CommandsLoadMiscellaneous,CommandsLoadFun, 
        AllowExpansions,AllowExpansionsV1,AllowExpansionsV2,AllowExpansionsV3,
        EnableModMenu,EnableConsole,EnableIl2CppDetourExceptionReporting,
        InjectMainMenuButtons,InjectRanchUIButtons,InjectPauseButtons,InjectTranslations,
        AddCheatMenuButton,AddModMenuButton,CheckForUpdates,AllowAutoUpdate,EnableInfHealth,
        EnableInfEnergy,EnableCheatMenu,EnableLocalizedVersionPatch,EnableThemeMenu,
        ChangeSystemContextIsModded,AllowPrism, AllowSaveExport, TryFixingInvalidSceneGroups
        
    };

    private static FeatureFlag[] extraDevFlags => new[] {
        DevMode, Experiments, CommandsLoadDevOnly, CommandsLoadExperimental, IgnoreSaveErrors, 
        ExperimentalKeyCodes, EnableRepoMenu, UseMockRepo, //InjectOptionsButtons, AddMockOptionsUIButtons
    };
    private static FeatureFlag[] extraBetaFlags => new []{None};
    private static FeatureFlag[] extraAlphaFlags => new []{None};
    
    private static List<FeatureFlag> flagsToForceOff = new List<FeatureFlag>();
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
    private static List<FeatureFlag> enabledFlags = new List<FeatureFlag>();
    
    static bool initialized = false;
    //internal static string flagfile_path = SR2EEntryPoint.instance.MelonAssembly.Assembly.Location+"/../../UserData/.sr2eflags.xml";

    
    internal static string flagPath => Path.Combine(SR2EEntryPoint.FlagDataPath,SR2EEntryPoint.updateBranch+".flags");
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
            if(!flagsToForceOff.HasFlag(flag))
            {
                xmlElement.SetAttribute("value", flag.HasFlag().ToString().ToLower());
                xmlElement.SetAttribute("default", flag.GetDefault().ToString().ToLower());
            }
            else
            {
                xmlElement.SetAttribute("value", "false");
                xmlElement.SetAttribute("default", flag.GetDefault().ToString().ToLower());
            }
            
            if (_requirementsMap.ContainsKey(flag)) if(_requirementsMap[flag].Length!=0)
                foreach (FFR req in _requirementsMap[flag])
                {
                    if (req == null) continue;
                    string name = req.GetType().Name;
                    if (req is FFRString ffrString)
                    {
                        string exist = xmlElement.GetAttribute(name);
                        if (!string.IsNullOrEmpty(exist)) exist += ",";
                        xmlElement.SetAttribute(name, exist+ffrString.String);
                    }
                    else if (req is FFRFlag ffrFlag)
                    {
                        string exist = xmlElement.GetAttribute(name);
                        if (!string.IsNullOrEmpty(exist)) exist += ",";
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
        
        if (File.Exists(flagPath)) File.SetAttributes(flagPath, FileAttributes.Normal);
        xmlDoc.Save(flagPath);
        File.SetAttributes(flagPath, FileAttributes.Hidden);
    }

    static void LoadFromFlagFile()
    {
        flagsToForceOff = new List<FeatureFlag>();
        var xmlDoc = new XmlDocument();
        
        if (!File.Exists(flagPath)) return; 
        
        try { xmlDoc.Load(flagPath); }
        catch {}

        XmlElement root = xmlDoc["SR2EFeatureFlags"];
        if (root == null) return; 
        
        XmlElement strings = root["FeatureStringValue"];
        if (strings != null)
        {
            if(strings["LAST_SR2EVERSION"]!=null)
            {
                if (strings["LAST_SR2EVERSION"].GetAttribute("value") != BuildInfo.DisplayVersion) return; 
            }
            foreach (XmlElement stringElement in strings.ChildNodes)
                if(stringElement.Name!="FeatureStringValue")
                    if (Enum.TryParse(stringElement.Name, out FeatureStringValue stringValue))
                        if (stringValue.GetDefault() == stringElement.GetAttribute("default")) 
                            featureStrings[stringValue] = stringElement.GetAttribute("value");
        }

        XmlElement flags = root["FeatureFlags"];
        if (flags != null)
            foreach (XmlElement flagElement in flags.ChildNodes)
                if (Enum.TryParse(flagElement.Name, out FeatureFlag flag))
                    if (flag.GetDefault().ToString().ToLower() == flagElement.GetAttribute("default"))
                        if (bool.TryParse(flagElement.GetAttribute("value"), out bool isEnabled))
                            flag.SetFlag(isEnabled);
                    
                
        XmlElement ints = root["FeatureIntegerValues"];
        if (ints != null)
            foreach (XmlElement intElement in ints.ChildNodes)
                if (Enum.TryParse(intElement.Name, out FeatureIntegerValue intValue))
                    if (intValue.GetDefault().ToString() == intElement.GetAttribute("default"))
                        if (int.TryParse(intElement.GetAttribute("value"), out int intResult))
                            featureInts[intValue] = intResult;

        
        foreach (FeatureFlag flag in Enum.GetValues(typeof(FeatureFlag)))
        {
            if(flag.requirementsMet()) continue;
            flag.DisableFlag();
            if(!flagsToForceOff.Contains(flag));
            flagsToForceOff.Add(flag);
        }
    }

    internal static void InitFlagManager()
    {
        if (initialized) return;
        initialized = true;
        enabledFlags = new List<FeatureFlag>();
        flagsToForceOff = new List<FeatureFlag>();
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
        try //Delete old flag files
        {
            foreach (var pair in MiscEUtil.BRANCHES)
                try { File.Delete(Application.persistentDataPath + "/"+pair.Value+".sr2eflags.xml"); } catch {}
        } catch { }
        try
        {
            if (File.Exists(flagPath)) LoadFromFlagFile();
            SaveToFlagFile();
        }
        catch { }
        

        string[] launchArgs = Environment.GetCommandLineArgs();
        var usedArgs = new List<String>();
        foreach (string arg in launchArgs)
        {
            if (arg.StartsWith("-sr2e.") && arg.Contains("="))
            {
                var split = arg.Split("=");
                if (split.Length != 2) continue;
                if (usedArgs.Contains(split[0])) continue;
                usedArgs.Add(split[0]);
                switch (split[0])
                {
                    case "-sr2e.forceredirectsaves":
                        if (split[1] == "true") EnableFlag(RedirectSaveFiles);
                        break;
                }
            }
        }
        
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
        
        if(DisableCheats.HasFlag())
            SR2ECounterGateManager.RegisterFor_DisableCheats(SR2EEntryPoint.instance);
        
    }
    public static CommandType enabledCommands => enabledCMDs;
    public static bool HasFlag(this FeatureFlag featureFlag) => enabledFlags.Contains(featureFlag);
    public static bool HasFlag(this bool[] array,FeatureFlag featureFlag) => array[Convert.ToInt32(featureFlag)];
    public static bool HasFlag(this FeatureFlag[] array,FeatureFlag featureFlag) => array.Contains(featureFlag);
    public static bool HasFlag(this List<FeatureFlag> list,FeatureFlag featureFlag) => list.Contains(featureFlag);

    static void SetFlag(this FeatureFlag featureFlag, bool state)
    {
        if (state) EnableFlag(featureFlag);
        else DisableFlag(featureFlag);
    }

    static bool EnableFlag(this FeatureFlag featureFlag)
    {
        if(!enabledFlags.Contains(featureFlag))
        {
            enabledFlags.Add(featureFlag);
            return true;
        }

        return false;
    }
    static bool DisableFlag(this FeatureFlag featureFlag) => enabledFlags.Remove(featureFlag);

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

    public static bool GetDefault(this FeatureFlag featureFlag) => defaultFlags.HasFlag(featureFlag);
    
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
        {AllowPrism,new FFR[]{new FFRActivated(InjectTranslations)}},
        {AllowExpansionsV1,new FFR[]{new FFRActivated(AllowExpansions)}},
        {AllowExpansionsV2,new FFR[]{new FFRActivated(AllowExpansions)}},
        {AllowExpansionsV3,new FFR[]{new FFRActivated(AllowExpansions)}},
        {AddMockMainMenuButtons, new FFR[]{new FFRActivated(InjectMainMenuButtons)}},
        {RestoreDebugFPSViewer, new FFR[]{new FFRActivated(RestoreDebugAbilities)}},
        {RestoreDebugPlayerDebug, new FFR[]{new FFRActivated(RestoreDebugAbilities)}},
        {RestoreDebugDevConsole, new FFR[]{new FFRActivated(RestoreDebugAbilities)}},
        {RestoreDebugDebugUI, new FFR[]{new FFRActivated(RestoreDebugAbilities)}},
        {IgnoreWorldPopulatorErrors, new FFR[]{new FFRActivated(ShowWorldPopulatorErrors)}},
        {InjectOptionsButtons, new FFR[]{new FFRActivated(Experiments)}}
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
    
    public static List<FeatureFlag> featureFlags => enabledFlags;
    [Obsolete("OBSOLETE!: Use SR2EFeatureFlags.featureFlags!")] public static bool[] flags => new bool[999999];
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



