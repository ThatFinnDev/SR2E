using Il2CppMonomiPark.ScriptedValue;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Key = SR2E.Enums.Key;

namespace SR2E;

public class SR2EConsole : SR2EMenu
{
    public new static MenuIdentifier GetMenuIdentifier() =>
        new MenuIdentifier("console", SR2EMenuTheme.Default, "Console");

    public new static void PreAwake(GameObject obj) => obj.AddComponent<SR2EConsole>();
    public override bool createCommands => true;
    public override bool inGameOnly => false;

    internal MultiKey openKey = new MultiKey(Key.Tab, Key.LeftControl);
    internal Transform consoleContent;
    TMP_InputField commandInput;
    GameObject autoCompleteEntryPrefab;
    Transform autoCompleteContent;
    GameObject autoCompleteScrollView;
    GameObject messagePrefab;
    int selectedAutoComplete = 0;
    List<string> commandHistory;
    int commandHistoryIdx = -1;
    Scrollbar _scrollbar;
    bool shouldResetTime = false;
    bool scrollCompletlyDown = false;
    private bool didHelloWorld = false;
    protected override void OnAwake()
    {
        SR2EEntryPoint.menus.Add(this, new Dictionary<string, object>()
        {
            { "requiredFeatures", new List<FeatureFlag>() { EnableConsole } },
            { "openActions", new List<MenuActions> { MenuActions.PauseGameFalse, MenuActions.DisableInput } },
            { "closeActions", new List<MenuActions> { MenuActions.UnPauseGameFalse, MenuActions.EnableInput } },
        });
    }

    void CheckHelloWorld()
    {
        if (didHelloWorld) return;
        didHelloWorld = true;
        SendMessage(translation("console.helloworld"),false);
    }

    internal void SendMessage(string message)
    {
        if (!EnableConsole.HasFlag()) return;
        if (!SR2EEntryPoint.menusFinished) return;
        if (!didHelloWorld) CheckHelloWorld();
        try
        {
            if (message.Contains("\n"))
            {
                foreach (string singularLine in message.Split('\n')) SendMessage(singularLine);
                return;
            }
            GameObject instance = Instantiate(messagePrefab, consoleContent);
            instance.gameObject.SetActive(true);
            instance.transform.GetChild(0).gameObject.SetActive(true);
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 1f);
            _scrollbar.value = 0f;
            scrollCompletlyDown = true;
        } catch { }
    }
    internal void SendError(string message)
    {
        if (!EnableConsole.HasFlag()) return;
        if (!SR2EEntryPoint.menusFinished) return;
        if (!didHelloWorld) CheckHelloWorld();
        try
        {
            if (message.Contains("\n"))
            {
                foreach (string singularLine in message.Split('\n')) SendError(singularLine);
                return;
            }
            GameObject instance = Instantiate(messagePrefab, consoleContent);
            instance.gameObject.SetActive(true);
            instance.transform.GetChild(0).gameObject.SetActive(true);
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 0f, 0f, 1f);
            _scrollbar.value = 0f;
            scrollCompletlyDown = true;
        } catch { }
    }
    internal void SendWarning(string message)
    {
        if (!EnableConsole.HasFlag()) return;
        if (!SR2EEntryPoint.menusFinished) return;
        if (!didHelloWorld) CheckHelloWorld();
        try
        {
            if (message.Contains("\n"))
            {
                foreach (string singularLine in message.Split('\n')) SendWarning(singularLine);
                return;
            }
            GameObject instance = Instantiate(messagePrefab, consoleContent);
            instance.gameObject.SetActive(true);
            instance.transform.GetChild(0).gameObject.SetActive(true);
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 0f, 1f);
            _scrollbar.value = 0f;
            scrollCompletlyDown = true;
        } catch { }
    }


    protected override void OnClose()
    {
        for (int i = 0; i < autoCompleteContent.childCount; i++)
        {
            Destroy(autoCompleteContent.GetChild(i).gameObject);
        }
    }

    protected override void OnOpen()
    {
        if (!didHelloWorld) CheckHelloWorld();
        RefreshAutoComplete(commandInput.text);
    }


    void RefreshAutoComplete(string text)
    {
        autoCompleteContent.parent.parent.GetComponent<ScrollRect>().enabled =
            true; // Make sure that the component is enabled            

        if (selectedAutoComplete > autoCompleteContent.childCount - 1)
            selectedAutoComplete = 0;
        for (int i = 0; i < autoCompleteContent.childCount; i++)
            Object.Destroy(autoCompleteContent.GetChild(i).gameObject);
        if (String.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (text.Contains(" "))
        {
            string cmd = text.Substring(0, text.IndexOf(' '));
            if (SR2ECommandManager.commands.ContainsKey(cmd))
            {
                var argString = text;
                List<string> split = argString.Split(' ').ToList();
                split.RemoveAt(0);
                int argIndex = split.Count - 1;
                string[] args = null;
                if (split.Count != 0)
                    args = split.ToArray();
                List<string> possibleAutoCompletes = (SR2ECommandManager.commands[cmd].GetAutoComplete(argIndex, args));
                if (possibleAutoCompletes != null)
                    if (possibleAutoCompletes.Count == 0)
                        possibleAutoCompletes = null;
                if (possibleAutoCompletes != null)
                {
                    int maxPredictions = MAX_AUTOCOMPLETE.Get();
                    int predicted = 0;
                    foreach (string argument in possibleAutoCompletes)
                    {
                        if (predicted > maxPredictions)
                            break;
                        string containing = "";
                        if (args != null) containing = split[split.Count - 1];
                        if (args != null)
                            if (!argument.ToLower().Contains(containing.ToLower()))
                                continue;
                        predicted++;
                        GameObject instance = Instantiate(autoCompleteEntryPrefab, autoCompleteContent);
                        TextMeshProUGUI textMesh = instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        if (args == null)
                            textMesh.text =
                                "<alpha=#FF>" + argument +
                                "<alpha=#65>"; // "alpha=#FF" is the normal argument, and the "alhpa=#65" is the uncompleted part. DO NOT CHANGE THE SYSTEM, ONLY THE ALPHA VALUES!!!
                        else
                            textMesh.text = new Regex(Regex.Escape(containing), RegexOptions.IgnoreCase).Replace(
                                argument,
                                "<alpha=#6F>" + argument.Substring(argument.ToLower().IndexOf(containing.ToLower()),
                                    containing.Length) + "<alpha=#45>", 1);

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
            foreach (KeyValuePair<string, SR2ECommand> valuePair in SR2ECommandManager.commands)
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


    protected override void OnLateAwake()
    {
        commandHistory = new List<string>();
        


        menuBlock = transform.parent.getObjRec<GameObject>("blockRec");
        consoleContent = transform.getObjRec<Transform>("ConsoleMenuConsoleContentRec");
        messagePrefab = transform.getObjRec<GameObject>("ConsoleMenuTemplateMessageRec");
        commandInput = transform.getObjRec<TMP_InputField>("ConsoleMenuCommandInputRec");
        _scrollbar = transform.getObjRec<Scrollbar>("ConsoleMenuConsoleScrollbarRec");
        autoCompleteContent = transform.getObjRec<Transform>("ConsoleMenuAutoCompleteContentRec");
        autoCompleteEntryPrefab = transform.getObjRec<GameObject>("ConsoleMenuTemplateAutoCompleteEntryRec");
        autoCompleteScrollView = transform.getObjRec<GameObject>("ConsoleMenuAutoCompleteScrollRectRec");
        autoCompleteScrollView.GetComponent<ScrollRect>().enabled = false;
        autoCompleteScrollView.SetActive(false);

        commandInput.onValueChanged.AddListener((Action<string>)((text) => { RefreshAutoComplete(text); }));

    }


    protected override void OnUpdate()
    {
        try { if (consoleContent.childCount >= MAX_CONSOLELINES.Get())
            Destroy(consoleContent.GetChild(0).gameObject);
        } catch { }

        commandInput.ActivateInputField();
        if (scrollCompletlyDown)
            if (_scrollbar.value != 0)
            {
                _scrollbar.value = 0f;
                scrollCompletlyDown = false;
            }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
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

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            if (commandInput.text != "")
                Execute();

        if (commandHistoryIdx != -1 && !autoCompleteScrollView.active)
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                commandInput.text = commandHistory[commandHistoryIdx];
                commandInput.MoveToEndOfLine(false, false);
                RefreshAutoComplete(commandInput.text);
                commandHistoryIdx -= 1;
                autoCompleteScrollView.SetActive(false);
            }
        }

        if (autoCompleteContent.childCount != 0 && autoCompleteScrollView.active)
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                NextAutoComplete();

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
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
                        ((744f / 1080f) * Screen.height) - (27 * MAX_AUTOCOMPLETEONSCREEN.Get()) +
                        (27 * selectedAutoComplete),
                        autoCompleteContent.position.z);

                else
                    autoCompleteContent.position = new Vector3(autoCompleteContent.position.x,
                        ((744f / 1080f) * Screen.height),
                        autoCompleteContent.position.z);
            }
        }
        catch
        {
        }
    }


    void NextAutoComplete()
    {
        if (!isOpen) return;
        selectedAutoComplete += 1;
        if (selectedAutoComplete > autoCompleteContent.childCount - 1)
        {
            selectedAutoComplete = 0;
            autoCompleteContent.GetChild(autoCompleteContent.childCount - 1).GetComponent<Image>().color =
                new Color32(0, 0, 0, 25);
            autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color =
                new Color32(255, 211, 0, 120);
        }
        else
        {
            autoCompleteContent.GetChild(selectedAutoComplete - 1).GetComponent<Image>().color =
                new Color32(0, 0, 0, 25);
            autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color =
                new Color32(255, 211, 0, 120);
        }
    }

    void PrevAutoComplete()
    {
        if (!isOpen) return;
        selectedAutoComplete -= 1;

        if (selectedAutoComplete < 0)
        {
            selectedAutoComplete = autoCompleteContent.childCount - 1;
            autoCompleteContent.GetChild(0).GetComponent<Image>().color = new Color32(0, 0, 0, 25);
            autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color =
                new Color32(255, 211, 0, 120);
        }
        else
        {
            autoCompleteContent.GetChild(selectedAutoComplete + 1).GetComponent<Image>().color =
                new Color32(0, 0, 0, 25);
            autoCompleteContent.GetChild(selectedAutoComplete).GetComponent<Image>().color =
                new Color32(255, 211, 0, 120);
        }
    }

    void Execute()
    {
        if (!EnableConsole.HasFlag()) return;
        string cmds = commandInput.text;
        commandHistory.Add(cmds);
        commandHistoryIdx = commandHistory.Count - 1;
        commandInput.text = "";
        for (int i = 0; i < autoCompleteContent.childCount; i++)
            Object.Destroy(autoCompleteContent.GetChild(i).gameObject);
        SR2ECommandManager.ExecuteByString(cmds);
    }


}
