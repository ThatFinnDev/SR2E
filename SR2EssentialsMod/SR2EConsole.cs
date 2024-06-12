using System;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppTMPro;
using SR2E.Commands;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SR2E
{
    internal static class ConsoleController
    {
        public static void ConsoleOpen()
        {
            
        }
    }

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
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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
                    GameObject instance = GameObject.Instantiate(specialMessagePrefab, consoleContent);
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
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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

                    GameObject instance = GameObject.Instantiate(specialMessagePrefab, consoleContent);
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
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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
                    GameObject instance = GameObject.Instantiate(specialMessagePrefab, consoleContent);
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
        { get { return transform.GetChild(0).gameObject.activeSelf; } }

        /// <summary>
        /// Closes the console
        /// </summary>
        public static void Close()
        {
            for (int i = 0; i < autoCompleteContent.childCount; i++)
            { Object.Destroy(autoCompleteContent.GetChild(i).gameObject); }

            consoleBlock.SetActive(false);
            consoleMenu.SetActive(false);
            try { SystemContext.Instance.SceneLoader.UnpauseGame(); } catch  { }
            Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Enable();

        }

        /// <summary>
        /// Opens the console
        /// </summary>
        public static void Open()
        {
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
            if (SR2EModMenu.isOpen) return;
            if (SR2ESaveManager.WarpManager.warpTo != null) return;

            consoleBlock.SetActive(true);
            consoleMenu.SetActive(true);
            try { SystemContext.Instance.SceneLoader.TryPauseGame(); } catch  { }
            Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Disable();
            RefreshAutoComplete(commandInput.text);
        }
        /// <summary>
        /// Toggles the console
        /// </summary>
        public static void Toggle()
        {
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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
        
        public static bool RegisterCommand(SR2Command cmd)
        {
            if (commands.ContainsKey(cmd.ID.ToLowerInvariant()))
            {
                SendMessage(translation("cmd.alreadyregistered",cmd.ID.ToLowerInvariant()));
                return false;
            }
            commands.Add(cmd.ID.ToLowerInvariant(), cmd);
            List<KeyValuePair<string, SR2Command>> myList = commands.ToList();

            myList.Sort(delegate (KeyValuePair<string, SR2Command> pair1, KeyValuePair<string, SR2Command> pair2) { return pair1.Key.CompareTo(pair2.Key); });
            commands = myList.ToDictionary(x => x.Key, x => x.Value);
            return true;
        }
        /// <summary>
        /// Registers multiple commands to be used in the console
        /// </summary>
        public static bool RegisterCommands(SR2Command[] cmds)
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
        public static bool UnRegisterCommand(SR2Command cmd)
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
        /// <summary>
        /// Execute a string as if it was a commandId with args
        /// </summary>
        public static void ExecuteByString(string input, bool silent = false)
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
                        bool canPlay = false;
                        if (!SR2EConsole.isOpen)
                            if (!SR2EModMenu.isOpen)
                                if (Time.timeScale != 0)
                                    canPlay = true;

                        if (!canPlay && commands[cmd].executeWhenConsoleIsOpen) canPlay = true;
                        if (!silent) canPlay = true;
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
                                    SR2Command command = commands[cmd];
                                    command.silent = silent;
                                    successful = command.Execute(stringArray);
                                    command.silent = false;
                                }
                                else
                                {
                                    SR2Command command = commands[cmd];
                                    command.silent = silent;
                                    successful = command.Execute(null);
                                    command.silent = false;
                                }
                            }
                        }
                        else if(canPlay)
                        {
                            SR2Command command = commands[cmd];
                            command.silent = silent;
                            successful = command.Execute(null);
                            command.silent = false;
                        }
                    }
                    else
                        if (isOpen)
                            if (!SR2EModMenu.isOpen)
                                SendError(translation("cmd.unknowncommand"));
                }
            }
                
        }



        internal static Transform transform;
        internal static GameObject gameObject;
        internal static Dictionary<string, SR2Command> commands = new Dictionary<string, SR2Command>();

        static List<string> commandHistory;
        static int commandHistoryIdx = -1;

        static MelonLogger.Instance mlog;
        static Scrollbar _scrollbar;
        static bool shouldResetTime = false;
        private static bool scrollCompletlyDown = false;

        static void RefreshAutoComplete(string text)
        {
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
            
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
                        int maxPredictions = MAX_AUTOCOMPLETE; 
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
                foreach (KeyValuePair<string, SR2Command> valuePair in commands)
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

        //Setup ModMenu
        static void SetupModMenu()
        {
            SR2EModMenu.parent = transform;
            SR2EModMenu.gameObject = transform.getObjRec<GameObject>("modMenu");
            SR2EModMenu.transform = transform.getObjRec<Transform>("modMenu");
            SR2EModMenu.Start();
        }
        static void SetupCommands()
        {
            RegisterCommand(new FloatCommand());
            RegisterCommand(new GiveCommand());
            RegisterCommand(new UtilCommand());;
            RegisterCommand(new BindCommand());
            RegisterCommand(new UnbindCommand());
            RegisterCommand(new SpawnCommand());
            RegisterCommand(new FastForwardCommand());
            RegisterCommand(new ClearCommand());
            RegisterCommand(new ClearInventoryCommand());
            RegisterCommand(new ModsCommand());
            RegisterCommand(new HelpCommand());
            RegisterCommand(new RefillInvCommand());
            RegisterCommand(new NewBucksCommand());
            RegisterCommand(new KillCommand());
            RegisterCommand(new KillAllCommand());
            RegisterCommand(new GiveGadgetCommand());
            RegisterCommand(new GiveBlueprintCommand());
            RegisterCommand(new GiveUpgradeCommand());
            RegisterCommand(new ReplaceCommand());
            RegisterCommand(new SpeedCommand());
            RegisterCommand(new GravityCommand());
            RegisterCommand(new RotateCommand());
            RegisterCommand(new MoveCommand());
            RegisterCommand(new WeatherCommand());
            RegisterCommand(new FlingCommand());
            RegisterCommand(new PartyCommand());
            RegisterCommand(new GraphicsCommand());
            RegisterCommand(new FreezeCommand());
            RegisterCommand(new NoClipCommand());
            RegisterCommand(new StrikeCommand());
            RegisterCommand(new FXPlayCommand());
            if(SR2EEntryPoint.spawnAny)
                RegisterCommand(new LoadAllAssetsCommand());
            //RegisterCommand(new TimeScaleCommand());
            RegisterCommand(new InfiniteHealthCommand());
            RegisterCommand(new InfiniteEnergyCommand());
            RegisterCommand(new ScaleCommand());
            RegisterCommands(new SR2Command[]{new WarpCommand(), new SetWarpCommand(), new DeleteWarpCommand(),new WarpListCommand()});
            RegisterCommands(new SR2Command[]{new ConsoleVisibilityCommands.OpenConsoleCommand(), new ConsoleVisibilityCommands.CloseConsoleCommand(), new ConsoleVisibilityCommands.ToggleConsoleCommand()});

        }

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
                    foreach (KeyValuePair<string,SR2Command> pair in commands)
                        pair.Value.OnGameCoreLoad();
                    break;
                case "PlayerCore":
                    foreach (KeyValuePair<string,SR2Command> pair in commands)
                        pair.Value.OnPlayerCoreLoad();
                    break;
                case "UICore":
                    foreach (KeyValuePair<string,SR2Command> pair in commands)
                        pair.Value.OnUICoreLoad();
                    break;
                case "MainMenuUI":
                    foreach (KeyValuePair<string,SR2Command> pair in commands)
                        pair.Value.OnMainMenuUILoad();
                    break;
            }
        }
        internal static void Start()
        {
            commandHistory = new List<string>();
            if(!SR2EEntryPoint._mSRMLIsInstalled)
                if (SR2EEntryPoint.syncConsole)
                    SetupConsoleSync();

            mlog = new MelonLogger.Instance("SR2E");
            
            consoleBlock = transform.getObjRec<GameObject>("consoleBlock");
            consoleMenu = transform.getObjRec<GameObject>("consoleMenu");
            consoleContent = transform.getObjRec<Transform>("ConsoleContent"); 
            messagePrefab = transform.getObjRec<GameObject>("messagePrefab");
            specialMessagePrefab = transform.getObjRec<GameObject>("specialMessagePrefab");
            commandInput = transform.getObjRec<TMP_InputField>("commandInput");
            _scrollbar = transform.getObjRec<Scrollbar>("ConsoleScroll");
            autoCompleteContent = transform.getObjRec<Transform>("AutoCompleteContent");
            autoCompleteEntryPrefab = transform.getObjRec<GameObject>("AutoCompleteEntry");
            autoCompleteScrollView = transform.getObjRec<GameObject>("AutoCompleteScroll");
            autoCompleteScrollView.GetComponent<ScrollRect>().enabled = false;
            autoCompleteScrollView.SetActive(false);
            consoleBlock.SetActive(false);
            consoleMenu.SetActive(false);
            if (!SR2EEntryPoint._mSRMLIsInstalled)
            {
                commandInput.onValueChanged.AddListener((Action<string>)((text) => { RefreshAutoComplete(text); })); 
            }
            SetupCommands();
            SR2ESaveManager.Start();

            SetupModMenu();
            if(syncedSetuped) SetupConsoleSync();
        }

        static MultiKey openKey = new MultiKey(new Key[] { Key.LeftCtrl, Key.Tab });
        static TMP_InputField commandInput;
        static GameObject autoCompleteEntryPrefab;
        static GameObject consoleBlock;
        static GameObject consoleMenu;
        internal static Transform consoleContent;
        static Transform autoCompleteContent;
        static GameObject autoCompleteScrollView;
        static GameObject messagePrefab;
        static GameObject specialMessagePrefab;
        private static int selectedAutoComplete = 0;
        const int maxEntryOnScreen = 6;
        internal static void Update()
        {
            if (!SR2EEntryPoint._mSRMLIsInstalled)
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

                    if (Key.Tab.kc().wasPressedThisFrame)
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

                if (Key.Enter.kc().wasPressedThisFrame)
                    if (commandInput.text != "")
                        Execute();

                if (commandHistoryIdx != -1 && !autoCompleteScrollView.active)
                {
                    if (Key.UpArrow.kc().wasPressedThisFrame)
                    {
                        commandInput.text = commandHistory[commandHistoryIdx];
                        commandInput.MoveToEndOfLine(false, false);
                        RefreshAutoComplete(commandInput.text);
                        commandHistoryIdx -= 1;
                        autoCompleteScrollView.SetActive(false);
                    }
                }

                if(openKey.wasPressedThisFrame)
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
                    if (Key.DownArrow.kc().wasPressedThisFrame)
                        NextAutoComplete();

                    if (Key.UpArrow.kc().wasPressedThisFrame)
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
                        if (selectedAutoComplete > maxEntryOnScreen)
                            autoCompleteContent.position = new Vector3(autoCompleteContent.position.x,
                                ((744f/1080f)*Screen.height) - (27 * maxEntryOnScreen) + (27 * selectedAutoComplete),
                                autoCompleteContent.position.z);

                        else
                            autoCompleteContent.position = new Vector3(autoCompleteContent.position.x, ((744f/1080f)*Screen.height),
                                autoCompleteContent.position.z);
                    }
                }
                catch { }

                try
                {
                    if (consoleContent.childCount >= MAX_CONSOLELINES) GameObject.Destroy(consoleContent.GetChild(0).gameObject);
                }
                catch  { }

            }

            //Console Commands Update
            foreach (KeyValuePair<string, SR2Command> pair in commands)
                pair.Value.Update();
        }
        

        static void NextAutoComplete()
        {
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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
            if (SR2EEntryPoint._mSRMLIsInstalled) return;
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