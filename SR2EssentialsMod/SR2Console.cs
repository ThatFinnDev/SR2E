using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppMonomiPark.SlimeRancher.UI.Plot;
using Il2CppTMPro;
using MelonLoader;
using SR2E.Commands;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SR2E
{
    public static class SR2Console
    {
        /// <summary>
        /// Display a message in the console
        /// </summary>
        public static void SendMessage(string message)
        {
            if(!SR2EMain.consoleFinishedCreating)
                return;
            Transform consoleContent = getObjRec<Transform>(transform, "ConsoleContent");
            if (consoleContent.childCount >= maxMessages)
                GameObject.Destroy(consoleContent.GetChild(0).gameObject);
            if (message.Contains("\n"))
            {
                foreach (string singularLine in message.Split('\n'))
                    SendMessage(singularLine);
                return;
            }
            var instance = GameObject.Instantiate(getObjRec<TextMeshProUGUI>(transform, "messagePrefab"), consoleContent);
            instance.gameObject.SetActive(true);
            instance.text = message;
            _scrollbar.value = 0f;
            scrollCompletlyDown = true;
        }
        /// <summary>
        /// Display an error in the console
        /// </summary>
        public static void SendError(string message)
        {
            if(!SR2EMain.consoleFinishedCreating)
                return;
            Transform consoleContent = getObjRec<Transform>(transform, "ConsoleContent");
            if (consoleContent.childCount >= maxMessages)
                GameObject.Destroy(consoleContent.GetChild(0).gameObject);
            if (message.Contains("\n"))
            {
                foreach (string singularLine in message.Split('\n'))
                    SendError(singularLine);
                return;
            }
            var instance = GameObject.Instantiate(getObjRec<TextMeshProUGUI>(transform, "messagePrefab"), consoleContent);
            instance.gameObject.SetActive(true);
            instance.text = message;
            instance.color = new Color(0.6f, 0, 0, 1);
            _scrollbar.value = 0f;
            scrollCompletlyDown = true;
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
            
            if (Object.FindObjectsOfType<MapUI>().Length != 0)
                return;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            if (normalTimeScale == 0)
                normalTimeScale = 1;
            Time.timeScale = normalTimeScale;
            Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Enable();

        }
        
        /// <summary>
        /// Opens the console
        /// </summary>
        public static void Open()
        { 
            if (SR2ModMenu.isOpen)
                return;
            
            Il2CppArrayBase<MapUI> allMapUIs = Object.FindObjectsOfType<MapUI>();
            for (int i = 0; i < allMapUIs.Count; i++)
                Object.Destroy(allMapUIs[i].gameObject);
            
            Il2CppArrayBase<LandPlotUIRoot> allPlotUIRoots = Object.FindObjectsOfType<LandPlotUIRoot>();
            for (int i = 0; i < allPlotUIRoots.Count; i++)
                allPlotUIRoots[i].Close();
            
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            normalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Disable();

        }
        /// <summary>
        /// Toggles the console
        /// </summary>
        public static void Toggle()
        {
            if(isOpen)
                Close();
            else
                Open();
        }
        /// <summary>
        /// Registers a command to be used in the console
        /// </summary>
        public static bool RegisterCommand(SR2CCommand cmd)
        {
            if (commands.ContainsKey(cmd.ID.ToLowerInvariant()))
            {
                SendMessage($"Trying to register command with id '<color=white>{cmd.ID.ToLowerInvariant()}</color>' but the ID is already registered!");
                return false;
            }
            commands.Add(cmd.ID.ToLowerInvariant(), cmd);
            List<KeyValuePair<string, SR2CCommand>> myList = commands.ToList();

            myList.Sort(delegate(KeyValuePair<string, SR2CCommand> pair1, KeyValuePair<string, SR2CCommand> pair2) { return pair1.Key.CompareTo(pair2.Key); });
            commands = myList.ToDictionary(x => x.Key, x => x.Value);
            return true;
        }

        
        
        

        internal static Transform transform;
        internal static GameObject gameObject;
        internal static Dictionary<string, SR2CCommand> commands = new Dictionary<string, SR2CCommand>();
        internal static T getObjRec<T> (Transform transform, string name)
        {
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child.gameObject.GetComponent<T>();
                }
                else if (child.childCount > 0)
                {
                    T potentionalAnswer = getObjRec<T>(child.transform, name);
                    if (potentionalAnswer != null)
                        return potentionalAnswer;
                }
            }
            return default;
        }
        static Scrollbar _scrollbar;
        static float normalTimeScale = 1f;
        const int maxMessages = 100;
        private static bool scrollCompletlyDown = false;
        
        internal static void Start()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            RegisterCommand(new GiveCommand());
            RegisterCommand(new ClearCommand());
            RegisterCommand(new ClearInventoryCommand());
            RegisterCommand(new ModsCommand());
            RegisterCommand(new HelpCommand());
            RegisterCommand(new RefillSlotsCommand());
            
            //Disabled do to not working yet
            //RegisterCommand(new NoClipCommand());
            
            bool hasInfiniteEnergyMod = false;
            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
            {
                if (melonBase.ID == "InfiniteEnergy")
                    hasInfiniteEnergyMod = true;
            }

            if (!hasInfiniteEnergyMod)
            {
                RegisterCommand(new InfiniteEnergyCommand());
            }

            _scrollbar = getObjRec<Scrollbar>(transform,"ConsoleScroll");
            
            
            //Setup Modmenu
            
            SR2ModMenu.parent = transform;
            SR2ModMenu.gameObject = transform.GetChild(4).gameObject;
            SR2ModMenu.transform = transform.GetChild(4);
            SR2ModMenu.Start();
        }
        internal static void Update()
        {
            if (SR2EMain.consoleFinishedCreating != true)
                return;

            getObjRec<TMP_InputField>(transform, "commandInput").ActivateInputField();
            if (isOpen)
            {
                if (scrollCompletlyDown)
                    if (_scrollbar.value != 0)
                    {
                        _scrollbar.value = 0f;
                        scrollCompletlyDown = false;
                    }
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                    if(getObjRec<TMP_InputField>(transform,"commandInput").text!="")
                        Execute();
                if (Time.timeScale!=0f)
                    Time.timeScale=0;
            }
            if (Keyboard.current.ctrlKey.wasPressedThisFrame)
                if(Keyboard.current.tabKey.isPressed)
                    Toggle();
            if (Keyboard.current.tabKey.wasPressedThisFrame)
                if(Keyboard.current.ctrlKey.isPressed)
                    Toggle();
            

            if (_scrollbar == null)
                _scrollbar =transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Scrollbar>();
            else
            {
                float value = Mouse.current.scroll.ReadValue().y;
                if (Mouse.current.scroll.ReadValue().y!=0)
                    _scrollbar.value = Mathf.Clamp(_scrollbar.value+((value > 0.01 ? 1.25f : value < -0.01 ? -1.25f : 0) * _scrollbar.size),0,1f);
            }
            
            //Modmenu
            SR2ModMenu.Update();
        }
        

        static void Execute()
        {
            string[] cmds = getObjRec<TMP_InputField>(transform,"commandInput").text.Split(';');
            getObjRec<TMP_InputField>(transform, "commandInput").text = "";
            foreach (string c in cmds)
                if (!String.IsNullOrEmpty(c))
                {
                    bool spaces = c.Contains(" ");
                    string cmd = spaces ? c.Substring(0, c.IndexOf(' ')) : c;
                
                    if (commands.ContainsKey(cmd))
                    {
                        string[] args = spaces ? c.Remove(0,(cmd + " ").ToCharArray().Length).Split(' ') : null;
                        bool successful = commands[cmd].Execute(args);
                    }
                    else
                        SendError("Unknown command. Please use '<color=white>help</color>' for available commands");
                }
        }
        
        
    }
}