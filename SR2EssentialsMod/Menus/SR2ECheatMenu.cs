using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppSystem.Linq;
using Il2CppTMPro;
using SR2E.Buttons;
using SR2E.Commands;
using SR2E.Components;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Menus;

public class SR2ECheatMenu
{
    public static MenuIdentifier menuIdentifier = new MenuIdentifier(true,"cheatmenu",SR2EMenuTheme.Default,"CheatMenu");
    internal static List<SR2ECheatMenuButton> cheatButtons = new List<SR2ECheatMenuButton>();
    internal static List<CheatMenuRefineryEntry> refineryEntries = new List<CheatMenuRefineryEntry>();
    internal static List<CheatMenuGadgetEntry> gadgetEntries = new List<CheatMenuGadgetEntry>();
    internal static List<CheatMenuSlot> cheatSlots = new List<CheatMenuSlot>();
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    internal static Transform cheatButtonContent;
    internal static Transform refineryContent;
    internal static Transform gadgetsContent;
    internal static Transform warpsContent;
    private static Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();
    public static bool isOpen
    {
        get { return gameObject == null ? false : gameObject.activeSelf; }
    }
    /// <summary>
    /// Closes the cheat menu
    /// </summary>
    public static void Close()
    {
        if (!isOpen) return;
        menuBlock.SetActive(false);
        gameObject.SetActive(false);
        gameObject.getObjRec<Button>("CheatMenuMainSelectionButtonRec").onClick.Invoke();


        TryUnHideMenus();
        TryUnPauseGame();
        TryEnableSR2Input();
    }


    /// <summary>
    /// Opens the cheat menu
    /// </summary>
    public static void Open()
    {
        if(!SR2EConsole.CheatsEnabled) return;
        if (isAnyMenuOpen) return;
        menuBlock.SetActive(true);
        gameObject.SetActive(true);
        TryPauseAndHide();
        //TryDisableSR2Input();
        //Refinery
        
        refineryContent.DestroyAllChildren();
        List<IdentifiableType> refineryItems = SceneContext.Instance.GadgetDirector._refineryTypeGroup.GetAllMembers().ToArray().ToList();
        foreach (IdentifiableType refineryItem in refineryItems)
        {
            GameObject entry = Object.Instantiate(refineryEntryTemplate, refineryContent);
            entry.SetActive(true);
            entry.AddComponent<CheatMenuRefineryEntry>();
            entry.GetComponent<CheatMenuRefineryEntry>().item = refineryItem;
            entry.GetComponent<CheatMenuRefineryEntry>().OnOpen();
            refineryEntries.Add(entry.GetComponent<CheatMenuRefineryEntry>());
        }
        //Gadgets
        
        gadgetsContent.DestroyAllChildren();
        List<IdentifiableType> gadgetItems = SceneContext.Instance.GadgetDirector._gadgetsGroup.GetAllMembers().ToArray().ToList();
        foreach (IdentifiableType gadgetItem in gadgetItems)
        {
            GameObject entry = Object.Instantiate(gadgetsEntryTemplate, gadgetsContent);
            entry.SetActive(true);
            entry.AddComponent<CheatMenuGadgetEntry>();
            entry.GetComponent<CheatMenuGadgetEntry>().item = gadgetItem;
            entry.GetComponent<CheatMenuGadgetEntry>().OnOpen();
            gadgetEntries.Add(entry.GetComponent<CheatMenuGadgetEntry>());
        }
        
        
        //Cheat Buttons
        cheatButtonContent.DestroyAllChildren();
        foreach (SR2ECheatMenuButton cheatButton in cheatButtons)
        {
            GameObject button = Object.Instantiate(buttonTemplate, cheatButtonContent);
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cheatButton.label;
            button.GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                cheatButton.action.Invoke();
            }));
            cheatButton.textInstance = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cheatButton.buttonInstance = button.GetComponent<Button>();
        }


        noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (SceneContext.Instance.Camera.GetComponent<NoClipComponent>() == null ? "off" : "on"));
        if (EnableInfHealth.HasFlag()) infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
        if (EnableInfEnergy.HasFlag()) infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        removeFogButton.textInstance.text = translation("cheatmenu.cheatbuttons.removeFog" + (removeFog? "on" : "off"));
        betterScreenshotButton.textInstance.text = translation("cheatmenu.cheatbuttons.betterScreenshot" + (betterScreenshot? "on" : "off"));

        
        //Warp Buttons
        warpsContent.DestroyAllChildren();
        foreach (KeyValuePair<string,SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps.OrderBy(x => x.Key))
        {
            GameObject button = Object.Instantiate(buttonTemplate, warpsContent);
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pair.Key;
            button.GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    Close();        
                    SR2EError error = pair.Value.WarpPlayerThere();
                    switch (error)
                    {
                        case SR2EError.TeleportablePlayerNull: SR2EConsole.SendError(translation("cmd.error.teleportableplayernull")); break;
                        case SR2EError.SRCharacterControllerNull: SR2EConsole.SendError(translation("cmd.error.srccnull")); break;
                    }
                }));
        }

        //Cheat Slots
        int i = -1;
        foreach (CheatMenuSlot slot in cheatSlots)
        {
            i++;
            slot.gameObject.SetActive(SceneContext.Instance.PlayerState.Ammo.Slots[i].IsUnlocked);
            slot.OnOpen();
        }
        
        foreach (var pair in toTranslate) pair.Key.SetText(translation(pair.Value));
        
    }

    /// <summary>
    /// Toggles the cheat menu
    /// </summary>
    public static void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
    internal static void Update()
    {
        if (isOpen)
        {
            if (Key.Escape.OnKeyPressed())
                Close();
        }
    }
    static GameObject buttonTemplate;
    static GameObject refineryEntryTemplate;
    static GameObject gadgetsEntryTemplate;
    internal static void Start()
    {
        cheatButtonContent = transform.getObjRec<Transform>("CheatMenuCheatButtonsContentRec");
        refineryContent = transform.getObjRec<Transform>("CheatMenuRefineryContentRec");
        warpsContent = transform.getObjRec<Transform>("CheatMenuWarpsContentRec");
        gadgetsContent = transform.getObjRec<Transform>("CheatMenuGadgetContentRec");
        buttonTemplate = transform.getObjRec<GameObject>("CheatMenuTemplateButton");
        refineryEntryTemplate = transform.getObjRec<GameObject>("CheatMenuRefineryTemplateEntry");
        gadgetsEntryTemplate = transform.getObjRec<GameObject>("CheatMenuGadgetsTemplateEntry");
        
        CheatButtons();
        cheatSlots.Add(transform.getObjRec<GameObject>("CheatMenuStatsSlot1Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.getObjRec<GameObject>("CheatMenuStatsSlot2Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.getObjRec<GameObject>("CheatMenuStatsSlot3Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.getObjRec<GameObject>("CheatMenuStatsSlot4Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.getObjRec<GameObject>("CheatMenuStatsSlot5Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.getObjRec<GameObject>("CheatMenuStatsSlot6Rec").AddComponent<CheatMenuSlot>());

        transform.getObjRec<GameObject>("CheatMenuStatsNewbucksRec").AddComponent<CheatMenuNewbucks>();
        
        var button1 = transform.getObjRec<Image>("CheatMenuMainSelectionButtonRec");
        button1.sprite = whitePillBg;
        var button2 = transform.getObjRec<Image>("CheatMenuRefinerySelectionButtonRec");
        button2.sprite = whitePillBg;
        var button3 = transform.getObjRec<Image>("CheatMenuGadgetsSelectionButtonRec");
        button3.sprite = whitePillBg;
        var button4 = transform.getObjRec<Image>("CheatMenuSpawnSelectionButtonRec");
        button4.sprite = whitePillBg;
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.main");
        toTranslate.Add(button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.refinery");
        toTranslate.Add(button3.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.gadgets");
        toTranslate.Add(button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.spawn");
        toTranslate.Add(transform.getObjRec<TextMeshProUGUI>("TitleTextRec"),"cheatmenu.title");
    }

    internal static SR2ECheatMenuButton noclipButton;
    internal static SR2ECheatMenuButton infEnergyButton;
    internal static SR2ECheatMenuButton infHealthButton;
    internal static SR2ECheatMenuButton removeFogButton;
    internal static SR2ECheatMenuButton betterScreenshotButton;
    internal static bool removeFog = false;
    internal static bool betterScreenshot = false;
    static void CheatButtons()
    {
        if (EnableInfEnergy.HasFlag()) infEnergyButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infenergyoff"),
            () =>
        {
            SR2EConsole.ExecuteByString("infenergy", true,true);
            infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        });
        if (EnableInfHealth.HasFlag()) infHealthButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infhealthoff"),
            () =>
        {
            SR2EConsole.ExecuteByString("infhealth", true,true);
            infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
            });
        removeFogButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.removeFogoff"),
            () =>
            {
                removeFog = !removeFog;
                removeFogButton.textInstance.text = translation("cheatmenu.cheatbuttons.removeFog" + (removeFog? "on" : "off"));
            });
        betterScreenshotButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.betterScreenshotoff"),
            () =>
            {
                betterScreenshot = !betterScreenshot;
                betterScreenshotButton.textInstance.text = translation("cheatmenu.cheatbuttons.betterScreenshot" + (betterScreenshot? "on" : "off"));
            });
        noclipButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.noclipoff"),
            () =>
            {
                SR2EConsole.ExecuteByString("noclip", true,true);
                noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (SceneContext.Instance.Camera.GetComponent<NoClipComponent>()!=null ? "on" : "off"));
            });
        new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.refillinv"), () => { SR2EConsole.ExecuteByString("refillinv", true,true); });
        
    }
}