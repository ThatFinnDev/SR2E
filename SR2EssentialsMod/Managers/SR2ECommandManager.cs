using System;
using System.Linq;
using SR2E.Commands;
using SR2E.Enums;

namespace SR2E.Managers;

public static class SR2ECommandManager
{
    internal static Dictionary<string, SR2ECommand> commands = new Dictionary<string, SR2ECommand>();
    internal static void Start()
    {
        SetupCommands();
    }
    internal static void Update()
    {
        foreach (KeyValuePair<string, SR2ECommand> pair in commands)
            pair.Value.Update();
    }
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var pair in commands) try { pair.Value.OnStandaloneEngagementPromptLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var pair in commands) try { pair.Value.OnPlayerCoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": 
                foreach (var pair in commands) try { pair.Value.OnUICoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } 
                if (!string.IsNullOrEmpty(SR2EEntryPoint.onSaveLoadCommand)) 
                    ExecuteByString(SR2EEntryPoint.onSaveLoadCommand);
                break;
            case "MainMenuUI":
                foreach (var pair in commands) try { pair.Value.OnMainMenuUILoad(); } catch (Exception e) { MelonLogger.Error(e); }
                if (!string.IsNullOrEmpty(SR2EEntryPoint.onMainMenuLoadCommand)) 
                    ExecuteByString(SR2EEntryPoint.onMainMenuLoadCommand);
                break; break;
            case "LoadScene": foreach (var pair in commands) try { pair.Value.OnLoadSceneLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
    }
    internal static void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var pair in commands) try { pair.Value.OnStandaloneEngagementPromptUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var pair in commands) try { pair.Value.OnPlayerCoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var pair in commands) try { pair.Value.OnUICoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var pair in commands) try { pair.Value.OnMainMenuUIUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var pair in commands) try { pair.Value.OnLoadSceneUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
    }
    internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var pair in commands) try { pair.Value.OnStandaloneEngagementPromptInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var pair in commands) try { pair.Value.OnPlayerCoreInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var pair in commands) try { pair.Value.OnUICoreInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var pair in commands) try { pair.Value.OnMainMenuUIInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var pair in commands) try { pair.Value.OnLoadSceneInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
    }
    static void SetupCommands()
    {
        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {
            var exporters = melonBase.MelonAssembly.Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SR2ECommand)) && !t.IsAbstract);
            foreach (Type type in exporters)
                try
                {
                    if(type == typeof(MenuVisibilityCommands.OpenCommand)) continue;
                    if(type == typeof(MenuVisibilityCommands.CloseCommand)) continue;
                    if(type == typeof(MenuVisibilityCommands.ToggleCommand)) continue;
                    SR2ECommand sr2Command = (SR2ECommand)Activator.CreateInstance(type);
                    if((enabledCommands & sr2Command.type) == sr2Command.type)
                    {
                        if (sr2Command is InfiniteHealthCommand && !EnableInfHealth.HasFlag()) continue;
                        if (sr2Command is InfiniteEnergyCommand && !EnableInfEnergy.HasFlag()) continue;
                        if (sr2Command.type.HasFlag(CommandType.DontLoad)) continue;
                        try { RegisterCommand(sr2Command); }
                        catch (Exception e) { MelonLogger.Error(e); }
                    }
                }
                catch (Exception e) { MelonLogger.Error(e); }
        }
        foreach (var expansion in SR2EEntryPoint.expansionsV1V2)
            try { expansion.LoadCommands(); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.LoadCommands(); }
            catch (Exception e) { MelonLogger.Error(e); }
        SR2ECallEventManager.ExecuteStandard(CallEvent.OnLoadCommands);
                
    }
    /// <summary>
    /// Registers a command to be used in the console<br />
    /// Returns true if the registration was successful<br />
    /// BE CAREFUL: Every command automatically gets registered if it doesn't have the CommandType.DontLoad type!
    /// </summary>
    /// <param name="cmd">The SR2ECommand which should be registered</param>
    /// <returns>bool</returns>
    public static bool RegisterCommand(SR2ECommand cmd)
    {
        if(cmd.ID.Contains(" "))
        {
            SR2ELogManager.SendMessage(translation("cmd.idcontainsspace", cmd.ID.ToLowerInvariant()));
            return false;
        }
        if (commands.ContainsKey(cmd.ID.ToLowerInvariant()))
        {
            SR2ELogManager.SendMessage(translation("cmd.alreadyregistered", cmd.ID.ToLowerInvariant()));
            return false;
        }

        commands.Add(cmd.ID.ToLowerInvariant(), cmd);
        List<KeyValuePair<string, SR2ECommand>> myList = commands.ToList();

        myList.Sort(delegate(KeyValuePair<string, SR2ECommand> pair1, KeyValuePair<string, SR2ECommand> pair2)
        {
            return pair1.Key.CompareTo(pair2.Key);
        });
        commands = myList.ToDictionary(x => x.Key, x => x.Value);
        return true;
    }

    /// <summary>
    /// Registers multiple commands<br />
    /// Returns true if the registration was successful for every command<br />
    /// BE CAREFUL: Every command automatically gets registered if it doesn't have the CommandType.DontLoad type!
    /// </summary>
    /// <param name="cmds">The SR2ECommands which should be registered</param>
    /// <returns>bool</returns>
    public static bool RegisterCommands(params SR2ECommand[] cmds)
    {
        bool successful = true;
        for (int i = 0; i < cmds.Length; i++)
        {
            bool didWork = RegisterCommand(cmds[i]);
            if (!didWork)
                successful = false;
        }
        return successful;
    }
    /// <summary>
    /// Unregisters multiple commands<br />
    /// Returns true if the unregistration was successful for every command
    /// </summary>
    /// <param name="cmds">The SR2ECommands which should be unregistered</param>
    /// <returns>bool</returns>
    public static bool UnRegisterCommands(params SR2ECommand[] cmds)
    {
        bool successful = true;
        for (int i = 0; i < cmds.Length; i++)
        {
            bool didWork = UnRegisterCommand(cmds[i]);
            if (!didWork)
                successful = false;
        }
        return successful;
    }
    /// <summary>
    /// Unregisters a command to be used in the console<br />
    /// Returns true if the unregistration was successful
    /// </summary>
    /// <param name="cmd">The SR2ECommand which should be unregistered</param>
    /// <returns>bool</returns>
    public static bool UnRegisterCommand(SR2ECommand cmd)
    {
        return UnRegisterCommand(cmd.ID);
    }

    /// <summary>
    /// Unregisters a command to be used in the console<br />
    /// Returns true if the unregistration was successful
    /// </summary>
    /// <param name="cmd">The SR2ECommand's key which should be unregistered</param>
    /// <returns>bool</returns>
    public static bool UnRegisterCommand(string cmd)
    {
        if (commands.ContainsKey(cmd.ToLowerInvariant()))
        {
            commands.Remove(cmd.ToLowerInvariant());
            return true;
        }
        SR2ELogManager.SendMessage(translation("cmd.notregistered", cmd.ToLowerInvariant()));
        return false;
    }

    public static Dictionary<string, List<Action<string[]>>> commandAddons = new Dictionary<string, List<Action<string[]>>>();

    /// <summary>
    /// Registers a command addon
    /// </summary>
    /// <param name="command">The command's key</param>
    /// <param name="action">The action which should be executed</param>
    public static void RegisterCommandAddon(string command, Action<string[]> action)
    {
        if (commandAddons.TryGetValue(command, out List<Action<string[]>> list))
            list.Add(action);
        else
            commandAddons.Add(command, new List<Action<string[]>> { action });
    }
    
    /// <summary>
    /// Executes a string as a command
    /// </summary>
    /// <param name="input">The string which should be executed</param>
    /// <param name="silent">Whether it should write messages to the console</param>
    public static void ExecuteByString(string input, bool silent = false)
    {
        ExecuteByString(input, silent, false);
    }

    /// <summary>
    /// Execute a string as if it was a commandId with args
    /// </summary>
    /// <param name="input">The string as the console command input</param>
    /// <param name="silent">If the command is supposed to be silent</param>
    /// <param name="alwaysPlay">If the command should NOT be silent</param>
    public static void ExecuteByString(string input, bool silent, bool alwaysPlay)
    {
        string[] cmds = input.Split(';');
        foreach (string cc in cmds)
        {
            string c = cc.TrimStart(' ');
            if (!string.IsNullOrWhiteSpace(c))
            {
                bool spaces = c.Contains(" ");
                string cmd = spaces ? c.Substring(0, c.IndexOf(' ')) : c;

                if (commands.ContainsKey(cmd))
                {
                    bool canPlay = false;
                    if (!MenuEUtil.isAnyMenuOpen)
                        if (Time.timeScale != 0)
                            canPlay = true;
                    if (!canPlay)
                    {
                        var openMenu = MenuEUtil.GetOpenMenu();
                        if (openMenu != null)
                        {   
                            Type openMenuType = openMenu.GetType();
                            foreach (var type in commands[cmd].execWhileMenuOpen)
                                if(openMenuType==type)
                                {
                                    canPlay = true;
                                    break;
                                }
                        }
                    }
                    if (!silent) canPlay = true;
                    if (alwaysPlay) canPlay = true;
                    bool successful = false;
                    if (spaces)
                    {
                        var argString = c.TrimEnd(' ') + " ";
                        List<string> split = argString.Split(' ').ToList();
                        split.RemoveAt(0);
                        split.RemoveAt(split.Count - 1);
                        if (canPlay)
                        {
                            string[] args = null;
                            if (split.Count != 0) args = split.ToArray();
                            var command = commands[cmd];
                            if (command.type.HasFlag(CommandType.Cheat) && SR2ECounterGateManager.disableCheats)
                            {
                                try { command.SendCheatsDisabled(); } catch (Exception e) { MelonLogger.Error($"Error in command SendCheatsDisabled!\n{e}"); }
                            }
                            else
                            {
                                command.silent = silent;
                                try { successful = command.Execute(args); } catch (Exception e) { MelonLogger.Error($"Error in command execution!\n{e}"); }

                                try
                                {
                                    if (commandAddons.TryGetValue(cmd, out List<Action<string[]>> list))
                                        foreach (var action in list)
                                            action(args);
                                }
                                catch (Exception e) { MelonLogger.Error($"Error in command extension execution!\n{e}"); }
                            
                                command.silent = false;
                            }
                        }
                    }
                    else if (canPlay)
                    {
                        SR2ECommand command = commands[cmd];
                        if (command.type.HasFlag(CommandType.Cheat) && SR2ECounterGateManager.disableCheats)
                        {
                            try { command.SendCheatsDisabled(); } catch (Exception e) { MelonLogger.Error($"Error in command SendCheatsDisabled!\n{e}"); }
                        }
                        else
                        {
                            command.silent = silent;
                            try { successful = command.Execute(null); } catch (Exception e) { MelonLogger.Error($"Error in command execution!\n{e}"); }

                            try
                            {
                                if (commandAddons.TryGetValue(cmd, out List<Action<string[]>> list))
                                    foreach (var action in list)
                                        action(null);
                            }
                            catch (Exception e) { MelonLogger.Error($"Error in command extension execution!\n{e}"); }
                            
                            command.silent = false;
                        }
                    }

                    if (DebugLogging.HasFlag()) MelonLogger.Msg($"Command success: {successful}");
                }
                else if (!silent)
                    if (MenuEUtil.isAnyMenuOpen)
                        SR2ELogManager.SendError(translation("cmd.unknowncommand"));
            }
        }

    }



}