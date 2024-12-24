using Il2CppMonomiPark.ScriptedValue;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Il2CppTMPro;
using SR2E.Commands;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Localization.Metadata;
using UnityEngine.UI;

namespace SR2E
{
    //Changed transform to parent
    //transform to consoleMenu
    //no fix every get child :/
    public static class SR2EConsole
    {
        /// <summary>
        /// Display a message in the console
        /// </summary>

        public static void SendMessage(string message)
        {
            SendMessage(message, SR2EEntryPoint.syncConsole);
        }
        /// <summary>
        /// Display a message in the console
        /// </summary>
        public static void SendMessage(string message, bool doMLLog, bool internal_logMLForSingleLine = true)
        {
            if (!EnableConsole.HasFlag()) return;
            if(String.IsNullOrEmpty(message))
                return;
            try
            {
                if (message.Contains("[SR2E]:")) return;
                if (message.StartsWith("[UnityExplorer]")) return;
                if (message.StartsWith("[]:")) return;
                if (!SR2EEntryPoint.consoleFinishedCreating) return;
                if (message.Contains("\n"))
                {
                    if (doMLLog) mlog.Msg(message);
                    foreach (string singularLine in message.Split('\n'))
                        SendMessage(singularLine, doMLLog, false);
                    return;
                }
                else
                {
                    if (doMLLog && internal_logMLForSingleLine) MelonLogger.Msg($"[SR2E]: {message}");
                    GameObject instance = GameObject.Instantiate(messagePrefab, consoleContent);
                    instance.gameObject.SetActive(true);
                    instance.transform.GetChild(0).gameObject.SetActive(true);
                    instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
                    instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 1f);
                    _scrollbar.value = 0f;
                    scrollCompletlyDown = true;
                    return;
                }
            }
            catch { }
        }
        /// <summary>
        /// Display an error in the console
        /// </summary>
        public static void SendError(string message)
        {
            SendError(message, SR2EEntryPoint.syncConsole);
        }
        /// <summary>
        /// Display an error in the console
        /// </summary>
        public static void SendError(string message, bool doMLLog, bool internal_logMLForSingleLine = true)
        {
            if (!EnableConsole.HasFlag()) return;
            if(String.IsNullOrEmpty(message))
                return;
            try
            {
                if (message.Contains("[SR2E]:")) return;
                if (message.StartsWith("[UnityExplorer]")) return;
                if (message.StartsWith("[]:")) return;
                if (!SR2EEntryPoint.consoleFinishedCreating) return;
                if (message.Contains("\n"))
                {
                    if (doMLLog) mlog.Error(message);
                    foreach (string singularLine in message.Split('\n'))
                        SendError(singularLine, doMLLog, false);
                    return;
                }
                else
                {
                    if (doMLLog && internal_logMLForSingleLine) MelonLogger.Error($"[SR2E]: {message}");

                    GameObject instance = GameObject.Instantiate(messagePrefab, consoleContent);
                    instance.gameObject.SetActive(true);
                    instance.transform.GetChild(0).gameObject.SetActive(true);
                    instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
                    instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 0f, 0, 1);
                    _scrollbar.value = 0f;
                    scrollCompletlyDown = true;
                    return;
                }
            }
            catch { }
        }
        /// <summary>
        /// Display an error in the console
        /// </summary>
        public static void SendWarning(string message)
        {
            SendWarning(message, SR2EEntryPoint.syncConsole);
        }
        /// <summary>
        /// Display an error in the console
        /// </summary>
        public static void SendWarning(string message, bool doMLLog, bool internal_logMLForSingleLine = true)
        {
            if (!EnableConsole.HasFlag()) return;
            if(String.IsNullOrEmpty(message))
                return;
            try
            {
                if (message.Contains("[SR2E]:")) return;
                if (message.StartsWith("[UnityExplorer]")) return;
                if (message.StartsWith("[]:")) return;
                if (!SR2EEntryPoint.consoleFinishedCreating) return;
                if (message.Contains("\n"))
                {
                    if (doMLLog) mlog.Msg(message, new Color32(255, 155, 0, 255));
                    foreach (string singularLine in message.Split('\n'))
                        SendWarning(singularLine, doMLLog, false);
                    return;
                }
                else
                {
                    if (doMLLog && internal_logMLForSingleLine) MelonLogger.Warning($"[SR2E]: {message}");
                    GameObject instance = GameObject.Instantiate(messagePrefab, consoleContent);
                    instance.gameObject.SetActive(true);
                    instance.transform.GetChild(0).gameObject.SetActive(true);
                    instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
                    instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 0, 1);
                    _scrollbar.value = 0f;
                    scrollCompletlyDown = true;
                    return;
                }
            }
            catch { }
        }

        /// <summary>
        /// Check if console is open
        /// </summary>
        public static bool isOpen
        {
            get
            {
                if (!EnableConsole.HasFlag()) return false;
                if(gameObject==null) return false;
                return gameObject.activeSelf;
            }
        }

        /// <summary>
        /// Closes the console
        /// </summary>
        public static void Close()
        {
            if (!EnableConsole.HasFlag()) return;

            consoleBlock.SetActive(false);
            gameObject.SetActive(false);
            for (int i = 0; i < autoCompleteContent.childCount; i++)
            { Object.Destroy(autoCompleteContent.GetChild(i).gameObject); }
            

            TryUnPauseGame(false);
            TryEnableSR2Input();

        }

        /// <summary>
        /// Opens the console
        /// </summary>
        public static void Open()
        {
            if (!EnableConsole.HasFlag()) return;
            if (isAnyMenuOpen) return;
            if (SR2ESaveManager.WarpManager.warpTo != null) return;
            consoleBlock.SetActive(true);
            gameObject.SetActive(true);
            TryPauseGame(false);

            TryDisableSR2Input();
            RefreshAutoComplete(commandInput.text);
        }
        /// <summary>
        /// Toggles the console
        /// </summary>
        public static void Toggle()
        {
            if (!EnableConsole.HasFlag()) return;
            if (SystemContext.Instance.SceneLoader.CurrentSceneGroup.name != "StandaloneStart" &&
                SystemContext.Instance.SceneLoader.CurrentSceneGroup.name != "CompanyLogo" &&
                SystemContext.Instance.SceneLoader.CurrentSceneGroup.name != "LoadScene")
            {
                if (isOpen)
                    Close();
                else
                    Open();
            }
        }
        /// <summary>
        /// Registers a command to be used in the console
        /// </summary>
        
        public static bool RegisterCommand(SR2ECommand cmd)
        {
            if (commands.ContainsKey(cmd.ID.ToLowerInvariant()))
            {
                SendMessage(translation("cmd.alreadyregistered",cmd.ID.ToLowerInvariant()));
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
            SendMessage(translation("cmd.notregistered",cmd.ToLowerInvariant()));
            return false;
        }

        public static void ExecuteByString(string input, bool silent = false)
        {
            ExecuteByString(input, silent, false);
        }
        /// <summary>
        /// Execute a string as if it was a commandId with args
        /// </summary>
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
                        bool isModMenuOpen = SR2EModMenu.isOpen;
                        bool isConsoleOpen = isOpen;
                        bool isCheatMenuOpen = SR2ECheatMenu.isOpen;
                        bool canPlay = false;
                        if (!isConsoleOpen)
                            if (!isModMenuOpen)
                                if (!isCheatMenuOpen)
                                    if (Time.timeScale != 0)
                                        canPlay = true;

                        if (!canPlay)
                        {
                            if (commands[cmd].execWhenIsOpenConsole && !isModMenuOpen && !isCheatMenuOpen)
                                canPlay = true;
                            if (commands[cmd].execWhenIsOpenModMenu && !isConsoleOpen && !isCheatMenuOpen)
                                canPlay = true;
                            if (commands[cmd].execWhenIsOpenCheatMenu && !isModMenuOpen && !isConsoleOpen)
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
                                    if (command.type.HasFlag(SR2ECommand.CommandType.Cheat) && !cheatsEnabledOnSave.Value)
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
                                    SR2ECommand command = commands[cmd];if (command.type.HasFlag(SR2ECommand.CommandType.Cheat) && !cheatsEnabledOnSave.Value)
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
                            if (command.type.HasFlag(SR2ECommand.CommandType.Cheat) && !cheatsEnabledOnSave.Value)
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
                    else
                        if(!silent)
                            if (isOpen)
                                if (!SR2EModMenu.isOpen)
                                    if (!SR2ECheatMenu.isOpen)
                                        SendError(translation("cmd.unknowncommand"));
                }
            }
                
        }



        internal static Transform parent;
        internal static Transform transform;
        internal static GameObject gameObject;
        internal static Dictionary<string, SR2ECommand> commands = new Dictionary<string, SR2ECommand>();

        static List<string> commandHistory;
        static int commandHistoryIdx = -1;

        static MelonLogger.Instance mlog;
        static Scrollbar _scrollbar;
        static bool shouldResetTime = false;
        private static bool scrollCompletlyDown = false;

        static void RefreshAutoComplete(string text)
        {
            if (!EnableConsole.HasFlag()) return;
            
            // autoCompleteContent.position = new Vector3(autoCompleteContent.position.x, ((744f/1080f)*Screen.height), autoCompleteContent.position.z);
            if (selectedAutoComplete > autoCompleteContent.childCount - 1)
                selectedAutoComplete = 0;
            for (int i = 0; i < autoCompleteContent.childCount; i++)
                Object.Destroy(autoCompleteContent.GetChild(i).gameObject);
            if (String.IsNullOrWhiteSpace(text))
            {  return; }
            if (text.Contains(" "))
            {
                string cmd = text.Substring(0, text.IndexOf(' '));
                if (commands.ContainsKey(cmd))
                {
                    var argString = text;
                    List<string> split = argString.Split(' ').ToList();
                    split.RemoveAt(0);
                    int argIndex = split.Count - 1;
                    string[] args = null;
                    if (split.Count != 0)
                        args = split.ToArray();
                    List<string> possibleAutoCompletes = (commands[cmd].GetAutoComplete(argIndex, args));
                    if (possibleAutoCompletes != null) if (possibleAutoCompletes.Count == 0) possibleAutoCompletes = null;
                    Color textColor = new Color(0.75f, 0.75f, 0.75f, 1f);
                    if (possibleAutoCompletes != null)
                    {
                        int maxPredictions = MAX_AUTOCOMPLETE.Get(); 
                        int predicted = 0;
                        foreach (string argument in possibleAutoCompletes)
                        {
                            if (predicted > maxPredictions)
                                return;
                            string containing = "";
                            if (args != null) containing = split[split.Count - 1];
                            if (args != null)
                                if (!argument.ToLower().Contains(containing.ToLower()))
                                    continue;
                            predicted++;
                            GameObject instance = Object.Instantiate(autoCompleteEntryPrefab, autoCompleteContent);
                            TextMeshProUGUI textMesh = instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                            textMesh.color = textColor;
                            if (args == null) textMesh.text = "<color=white>"+argument+"</color>";
                            else textMesh.text = new Regex(Regex.Escape(containing), RegexOptions.IgnoreCase).Replace(argument, "<color=white>" + argument.Substring(argument.ToLower().IndexOf(containing.ToLower()),containing.Length) + "</color>", 1);
                            
                            instance.SetActive(true);
                            instance.GetComponent<Button>().onClick.AddListener((Action)(() =>
                            {
                                commandInput.text = cmd;

                                if (args != null)
                                {
                                    for (int i = 0; i < args.Length - 1; i++)
                                    {
                                        commandInput.text += " " + args[i];
                                    }

                                    commandInput.text += " " + argument;
                                }

                                commandInput.MoveToEndOfLine(false, false);
                            }));
                        }
                    }
                }
            }
            else
                foreach (KeyValuePair<string, SR2ECommand> valuePair in commands)
                    if (valuePair.Key.StartsWith(text) && !valuePair.Value.Hidden)
                    {
                        GameObject instance = Object.Instantiate(autoCompleteEntryPrefab, autoCompleteContent);
                        instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = valuePair.Key;
                        instance.SetActive(true);
                        instance.GetComponent<Button>().onClick.AddListener((Action)(() =>
                        {
                            commandInput.text = valuePair.Key;
                            commandInput.MoveToEndOfLine(false, false);
                        }));
                    }
            autoCompleteScrollView.SetActive(autoCompleteContent.childCount != 0);

        }

        static void SetupOtherMenus()
        {
            SR2EModMenu.parent = parent;
            SR2EModMenu.gameObject = parent.getObjRec<GameObject>("modMenu");
            SR2EModMenu.transform = parent.getObjRec<Transform>("modMenu");
            SR2EModMenu.Start();
            SR2ECheatMenu.parent = parent;
            SR2ECheatMenu.gameObject = parent.getObjRec<GameObject>("cheatMenu");
            SR2ECheatMenu.transform = parent.getObjRec<Transform>("cheatMenu");
            SR2ECheatMenu.Start();
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

        public static ScriptedBool cheatsEnabledOnSave;
        
        internal static bool syncedSetuped = false;
        
        static void SetupConsoleSync()
        {
            syncedSetuped = true;
            MelonLogger.MsgDrawingCallbackHandler += (c1, c2, s1, s2) => SendMessage($"[{s1}]: {s2}", false);
            MelonLogger.ErrorCallbackHandler += (s, s1) => SendError($"[{s}]: {s1}", false);
            MelonLogger.WarningCallbackHandler += (s, s1) => SendWarning($"[{s}]: {s}", false);
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
        internal static void Start()
        {
            gameObject = parent.getObjRec<GameObject>("consoleMenu");
            transform = parent.getObjRec<Transform>("consoleMenu");
            
            commandHistory = new List<string>();
            if (EnableConsole.HasFlag())
                if (SR2EEntryPoint.syncConsole)
                    SetupConsoleSync();

            mlog = new MelonLogger.Instance("SR2E");
            
            consoleBlock = parent.getObjRec<GameObject>("consoleBlockRec");
            consoleContent = transform.getObjRec<Transform>("ConsoleMenuConsoleContentRec"); 
            messagePrefab = transform.getObjRec<GameObject>("ConsoleMenuTemplateMessageRec");
            commandInput = transform.getObjRec<TMP_InputField>("ConsoleMenuCommandInputRec");
            _scrollbar = transform.getObjRec<Scrollbar>("ConsoleMenuConsoleScrollbarRec");
            autoCompleteContent = transform.getObjRec<Transform>("ConsoleMenuAutoCompleteContentRec");
            autoCompleteEntryPrefab = transform.getObjRec<GameObject>("ConsoleMenuTemplateAutoCompleteEntryRec");
            autoCompleteScrollView = transform.getObjRec<GameObject>("ConsoleMenuAutoCompleteScrollRectRec");
            autoCompleteScrollView.GetComponent<ScrollRect>().enabled = false;
            autoCompleteScrollView.SetActive(false);
            
            if (EnableConsole.HasFlag())
            {
                commandInput.onValueChanged.AddListener((Action<string>)((text) => { RefreshAutoComplete(text); })); 
            }
            SetupCommands();
            SR2ESaveManager.Start();

            SetupOtherMenus();
            foreach (Transform child in parent.GetChildren())
                child.gameObject.SetActive(false);
            if(syncedSetuped) SetupConsoleSync();
        }

        static MultiKey openKey = new MultiKey( Key.Tab,Key.LeftControl);
        static TMP_InputField commandInput;
        static GameObject autoCompleteEntryPrefab;
        static GameObject consoleBlock;
        internal static Transform consoleContent;
        static Transform autoCompleteContent;
        static GameObject autoCompleteScrollView;
        static GameObject messagePrefab;
        private static int selectedAutoComplete = 0;
        internal static void Update()
        {
            if (EnableConsole.HasFlag())
            {
                
                if (SR2EEntryPoint.consoleFinishedCreating != true)
                    return;
                commandInput.ActivateInputField();
                if (isOpen)
                {
                    if (scrollCompletlyDown)
                        if (_scrollbar.value != 0)
                        {
                            _scrollbar.value = 0f;
                            scrollCompletlyDown = false;
                        }

                    if (Key.Tab.OnKeyPressed())
                    {
                        if (autoCompleteContent.childCount != 0)
                            try
                            {
                                autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Button>().onClick
                                    .Invoke();
                                selectedAutoComplete = 0;
                            }
                            catch
                            {
                            }

                    }
                }

                if (Key.Enter.OnKeyPressed())
                    if (commandInput.text != "")
                        Execute();

                if (commandHistoryIdx != -1 && !autoCompleteScrollView.active)
                {
                    if (Key.UpArrow.OnKeyPressed())
                    {
                        commandInput.text = commandHistory[commandHistoryIdx];
                        commandInput.MoveToEndOfLine(false, false);
                        RefreshAutoComplete(commandInput.text);
                        commandHistoryIdx -= 1;
                        autoCompleteScrollView.SetActive(false);
                    }
                }

                if(openKey.OnKeyPressed())
                    Toggle();
                /*
                if (Keyboard.current.ctrlKey.wasPressedThisFrame)
                    if (Keyboard.current.tabKey.isPressed)
                        Toggle();
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                    if (Keyboard.current.ctrlKey.isPressed)
                        Toggle();*/
                if (autoCompleteContent.childCount != 0 && autoCompleteScrollView.active)
                {
                    if (Key.DownArrow.OnKeyPressed())
                        NextAutoComplete();

                    if (Key.UpArrow.OnKeyPressed())
                        PrevAutoComplete();
                }

                if (selectedAutoComplete == autoCompleteContent.childCount)
                {
                    selectedAutoComplete = 0;
                }

                if (_scrollbar != null)
                {
                    float value = Mouse.current.scroll.ReadValue().y;
                    if (Mouse.current.scroll.ReadValue().y != 0)
                        _scrollbar.value =
                            Mathf.Clamp(
                                _scrollbar.value + ((value > 0.01 ? 1.25f : value < -0.01 ? -1.25f : 0) *
                                                    _scrollbar.size), 0, 1f);

                }

                try
                {
                    if (autoCompleteContent.childCount != 0)
                    {
                        autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color =
                            new Color32(255, 211, 0, 120);
                        if (selectedAutoComplete > MAX_AUTOCOMPLETEONSCREEN.Get())
                            autoCompleteContent.position = new Vector3(autoCompleteContent.position.x,
                                ((744f/1080f)*Screen.height) - (27 * MAX_AUTOCOMPLETEONSCREEN.Get()) + (27 * selectedAutoComplete),
                                autoCompleteContent.position.z);

                        else
                            autoCompleteContent.position = new Vector3(autoCompleteContent.position.x, ((744f/1080f)*Screen.height),
                                autoCompleteContent.position.z);
                    }
                }
                catch { }

                try
                {
                    if (consoleContent.childCount >= MAX_CONSOLELINES.Get()) GameObject.Destroy(consoleContent.GetChild(0).gameObject);
                }
                catch  { }

            }

            //Console Commands Update
            foreach (KeyValuePair<string, SR2ECommand> pair in commands)
                pair.Value.Update();
        }
        

        static void NextAutoComplete()
        {
            if (!EnableConsole.HasFlag()) return;
            selectedAutoComplete += 1;
            if (selectedAutoComplete > autoCompleteContent.childCount - 1)
            {
                selectedAutoComplete = 0;
                autoCompleteContent.GetChild(autoCompleteContent.childCount - 1).GetComponent<Image>().color = new Color32(0, 0, 0, 25);
                autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color = new Color32(255, 211, 0, 120);
            }
            else
            {
                autoCompleteContent.GetChild(selectedAutoComplete - 1).GetComponent<Image>().color = new Color32(0, 0, 0, 25);
                autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color = new Color32(255, 211, 0, 120);
            }
        }
        static void PrevAutoComplete()
        {
            if (!EnableConsole.HasFlag()) return;
            selectedAutoComplete -= 1;

            if (selectedAutoComplete < 0)
            {
                selectedAutoComplete = autoCompleteContent.childCount - 1;
                autoCompleteContent.GetChild(0).GetComponent<Image>().color = new Color32(0, 0, 0, 25);
                autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color = new Color32(255, 211, 0, 120);
            }
            else
            {
                autoCompleteContent.GetChild(selectedAutoComplete + 1).GetComponent<Image>().color = new Color32(0, 0, 0, 25);
                autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color = new Color32(255, 211, 0, 120);
            }
        }

        static void Execute()
        {
            if (!EnableConsole.HasFlag()) return;
            string cmds = commandInput.text;
            commandHistory.Add(cmds);
            commandHistoryIdx = commandHistory.Count - 1;
            commandInput.text = "";
            for (int i = 0; i < autoCompleteContent.childCount; i++)
                Object.Destroy(autoCompleteContent.GetChild(i).gameObject);
            ExecuteByString(cmds);

        }


    }
}