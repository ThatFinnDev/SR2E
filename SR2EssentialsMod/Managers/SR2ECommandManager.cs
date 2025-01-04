using System;
using System.Linq;
using SR2E.Commands;
using SR2E.Menus;

namespace SR2E.Managers;

public static class SR2ECommandManager
{
    internal static void Start()
    {
        SetupCommands();
    }
    internal static void Update()
    {
        foreach (KeyValuePair<string, SR2ECommand> pair in SR2ECommandManager.commands)
            pair.Value.Update();
    }
    
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "GameCore":
                foreach (KeyValuePair<string,SR2ECommand> pair in commands)
                    pair.Value.OnGameCoreLoad();
                break;
            case "PlayerCore":
                foreach (KeyValuePair<string,SR2ECommand> pair in commands)
                    pair.Value.OnPlayerCoreLoad();
                break;
            case "UICore":
                foreach (KeyValuePair<string,SR2ECommand> pair in commands)
                    pair.Value.OnUICoreLoad();
                if (!System.String.IsNullOrEmpty(SR2EEntryPoint.onSaveLoadCommand)) ExecuteByString(SR2EEntryPoint.onSaveLoadCommand);
                break;
            case "MainMenuUI":
                foreach (KeyValuePair<string,SR2ECommand> pair in commands)
                    pair.Value.OnMainMenuUILoad();
                if (!System.String.IsNullOrEmpty(SR2EEntryPoint.onMainMenuLoadCommand)) ExecuteByString(SR2EEntryPoint.onMainMenuLoadCommand);
                break;
        }
    }

    static void SetupCommands()
    {
        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {
            IEnumerable<SR2ECommand> exporters = melonBase.MelonAssembly.Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SR2ECommand)) && !t.IsAbstract)
                .Select(t => (SR2ECommand)Activator.CreateInstance(t));
            foreach (SR2ECommand sr2Command in exporters)
                if(sr2Command!=null) 
                    if((enabledCommands & sr2Command.type) == sr2Command.type)
                    {
                        if (sr2Command is InfiniteHealthCommand && EnableInfHealth.HasFlag()) continue;
                        if (sr2Command is InfiniteEnergyCommand && EnableInfEnergy.HasFlag()) continue;
                        RegisterCommand(sr2Command);
                    }
        }
            
            
            
        foreach (var expansion in SR2EEntryPoint.expansions)
            try { expansion.LoadCommands(); }
            catch (Exception e) { MelonLogger.Error(e); }
                
    }
        /// <summary>
        /// Registers a command to be used in the console
        /// </summary>
        
        public static bool RegisterCommand(SR2ECommand cmd)
        {
            if (commands.ContainsKey(cmd.ID.ToLowerInvariant()))
            {
                SR2EConsole.SendMessage(translation("cmd.alreadyregistered",cmd.ID.ToLowerInvariant()));
                return false;
            }
            commands.Add(cmd.ID.ToLowerInvariant(), cmd);
            List<KeyValuePair<string, SR2ECommand>> myList = commands.ToList();

            myList.Sort(delegate (KeyValuePair<string, SR2ECommand> pair1, KeyValuePair<string, SR2ECommand> pair2) { return pair1.Key.CompareTo(pair2.Key); });
            commands = myList.ToDictionary(x => x.Key, x => x.Value);
            return true;
        }
        
        /// <summary>
        /// Registers multiple commands to be used in the console
        /// </summary>
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
        /// Unregisters a command
        /// </summary>
        public static bool UnRegisterCommand(SR2ECommand cmd)
        {
            return UnRegisterCommand(cmd.ID);
        }
        /// <summary>
        /// Unregisters a command
        /// </summary>
        public static bool UnRegisterCommand(string cmd)
        {
            if (commands.ContainsKey(cmd.ToLowerInvariant()))
            {
                commands.Remove(cmd.ToLowerInvariant());
                return true;
            }
            SR2EConsole.SendMessage(translation("cmd.notregistered",cmd.ToLowerInvariant()));
            return false;
        }

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
                if (!String.IsNullOrWhiteSpace(c))
                {
                    bool spaces = c.Contains(" ");
                    string cmd = spaces ? c.Substring(0, c.IndexOf(' ')) : c;

                    if (commands.ContainsKey(cmd))
                    {
                        bool isModMenuOpen = GM<SR2EModMenu>().isOpen;
                        bool isThemeMenuOpen = GM<SR2EThemeMenu>().isOpen;
                        bool isConsoleOpen = GM<SR2EConsole>().isOpen;
                        bool isCheatMenuOpen = GM<SR2ECheatMenu>().isOpen;
                        bool canPlay = false;
                        if (!isConsoleOpen)
                            if (!isModMenuOpen)
                                if (!isCheatMenuOpen)
                                    if (Time.timeScale != 0)
                                        canPlay = true;

                        if (!canPlay)
                        {
                            if (commands[cmd].execWhenIsOpenConsole && !isModMenuOpen && !isCheatMenuOpen && !isThemeMenuOpen)
                                canPlay = true;
                            if (commands[cmd].execWhenIsOpenModMenu && !isConsoleOpen && !isCheatMenuOpen && !isThemeMenuOpen)
                                canPlay = true;
                            if (commands[cmd].execWhenIsOpenCheatMenu && !isModMenuOpen && !isConsoleOpen && !isThemeMenuOpen)
                                canPlay = true;
                            if (commands[cmd].execWhenIsOpenThemeMenu && !isCheatMenuOpen && !isConsoleOpen && !isThemeMenuOpen)
                                canPlay = true;
                        }
                        if (!silent) canPlay = true;
                        if (alwaysPlay) canPlay = true;
                        bool successful;
                        if (spaces)
                        {
                            
                            var argString = c.TrimEnd(' ') + " ";
                            List<string> split = argString.Split(' ').ToList();
                            split.RemoveAt(0);
                            split.RemoveAt(split.Count - 1);
                            if (canPlay)
                            {
                                if (split.Count != 0)
                                {
                                    string[] stringArray = split.ToArray();
                                    SR2ECommand command = commands[cmd];
                                    if (command.type.HasFlag(CommandType.Cheat) && DisableCheats.HasFlag())
                                    {
                                        command.SendError(translation("cmd.cheatsdisabled"));
                                        successful = false;
                                    }
                                    else
                                    {
                                        command.silent = silent;
                                        successful = command.Execute(stringArray);
                                        command.silent = false;
                                    }
                                }
                                else
                                {
                                    SR2ECommand command = commands[cmd];if (command.type.HasFlag(CommandType.Cheat) && DisableCheats.HasFlag())
                                    {
                                        command.SendError(translation("cmd.cheatsdisabled"));
                                        successful = false;
                                    }
                                    else
                                    {
                                        command.silent = silent;
                                        successful = command.Execute(null);
                                        command.silent = false;
                                    }
                                }
                            }
                        }
                        else if(canPlay)
                        {
                            SR2ECommand command = commands[cmd];
                            if (command.type.HasFlag(CommandType.Cheat) && DisableCheats.HasFlag())
                            {
                                command.SendError(translation("cmd.cheatsdisabled"));
                                successful = false;
                            }
                            else
                            {
                                command.silent = silent;
                                successful = command.Execute(null);
                                command.silent = false;
                            }
                        }
                    }
                    else if(!silent)
                        if (isAnyMenuOpen)
                            SR2EConsole.SendError(translation("cmd.unknowncommand"));
                }
            }
                
        }



        internal static Dictionary<string, SR2ECommand> commands = new Dictionary<string, SR2ECommand>();

}