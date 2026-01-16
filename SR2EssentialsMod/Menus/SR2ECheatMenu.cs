using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppSystem.Linq;
using Il2CppTMPro;
using SR2E.Buttons;
using SR2E.Commands;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Enums.Sounds;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E.Menus;

public class SR2ECheatMenu : SR2EMenu
{
    public new static MenuIdentifier GetMenuIdentifier() => new MenuIdentifier("cheatmenu",SR2EMenuFont.SR2,SR2EMenuTheme.Default,"CheatMenu");
    public override bool createCommands => true;
    public override bool inGameOnly => true;
    
    protected override void OnAwake()
    {
        requiredFeatures = new List<FeatureFlag>() { EnableCheatMenu }.ToArray();
        openActions = new List<MenuActions> { MenuActions.PauseGame, MenuActions.HideMenus }.ToArray();
        closeActions = new List<MenuActions> { MenuActions.UnPauseGame, MenuActions.UnHideMenus, MenuActions.EnableInput }.ToArray();
    }
    
    
    internal static List<SR2ECheatMenuButton> cheatButtons = new List<SR2ECheatMenuButton>();
    List<CheatMenuRefineryEntry> refineryEntries = new List<CheatMenuRefineryEntry>();
    List<CheatMenuGadgetEntry> gadgetEntries = new List<CheatMenuGadgetEntry>();
    List<CheatMenuSlot> cheatSlots = new List<CheatMenuSlot>();
    Transform cheatButtonContent;
    Transform refineryContent;
    Transform gadgetsContent;
    Transform warpsContent;
    GameObject buttonTemplate;
    GameObject refineryEntryTemplate;
    GameObject gadgetsEntryTemplate;
    SR2ECheatMenuButton refillButton;
    SR2ECheatMenuButton noclipButton;
    SR2ECheatMenuButton infEnergyButton;
    SR2ECheatMenuButton infHealthButton;
    SR2ECheatMenuButton removeFogButton;
    SR2ECheatMenuButton betterScreenshotButton;
    internal static bool removeFog = false;
    internal static bool betterScreenshot = false;
    
    protected override void OnClose()
    {
        gameObject.GetObjectRecursively<Button>("CheatMenuMainSelectionButtonRec").onClick.Invoke();
        refineryContent.DestroyAllChildren();
        gadgetsContent.DestroyAllChildren();
        cheatButtonContent.DestroyAllChildren();
        warpsContent.DestroyAllChildren();
    }
    
    protected override void OnOpen()
    {
        if(SR2ECounterGateManager.disableCheats)
        {
            Close();
            return;
        }
        //Refinery
        List<IdentifiableType> refineryItems = sceneContext.GadgetDirector._refineryTypeGroup.GetAllMembers().ToArray().ToList();
        refineryItems.Sort((x, y) => string.Compare(x.GetName(), y.GetName(), StringComparison.OrdinalIgnoreCase));
        foreach (IdentifiableType refineryItem in refineryItems)
        {
            GameObject entry = Instantiate(refineryEntryTemplate, refineryContent);
            entry.SetActive(true);
            entry.AddComponent<CheatMenuRefineryEntry>();
            entry.GetComponent<CheatMenuRefineryEntry>().item = refineryItem;
            entry.GetComponent<CheatMenuRefineryEntry>().OnOpen();
            refineryEntries.Add(entry.GetComponent<CheatMenuRefineryEntry>());
        }
        //Gadgets

        List<IdentifiableType> gadgetItems = sceneContext.GadgetDirector._gadgetsGroup.GetAllMembers().ToArray().ToList();
        gadgetItems.Sort((x, y) => string.Compare(x.GetName(), y.GetName(), StringComparison.OrdinalIgnoreCase));
        foreach (IdentifiableType gadgetItem in gadgetItems)
        {
            GameObject entry = Instantiate(gadgetsEntryTemplate, gadgetsContent);
            entry.SetActive(true);
            entry.AddComponent<CheatMenuGadgetEntry>();
            entry.GetComponent<CheatMenuGadgetEntry>().item = gadgetItem;
            entry.GetComponent<CheatMenuGadgetEntry>().OnOpen();
            gadgetEntries.Add(entry.GetComponent<CheatMenuGadgetEntry>());
        }
        
        
        //Cheat Buttons
        foreach (SR2ECheatMenuButton cheatButton in cheatButtons)
        {
            GameObject button = Instantiate(buttonTemplate, cheatButtonContent);
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cheatButton.label;
            button.GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                cheatButton.action.Invoke();
            }));
            cheatButton.textInstance = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cheatButton.buttonInstance = button.GetComponent<Button>();
        }


        noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (sceneContext.Camera.GetComponent<NoClipComponent>() == null ? "off" : "on"));
        refillButton.textInstance.text = translation("cheatmenu.cheatbuttons.refillinv");
        if (EnableInfHealth.HasFlag()) infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
        if (EnableInfEnergy.HasFlag()) infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        removeFogButton.textInstance.text = translation("cheatmenu.cheatbuttons.removeFog" + (removeFog? "on" : "off"));
        betterScreenshotButton.textInstance.text = translation("cheatmenu.cheatbuttons.betterScreenshot" + (betterScreenshot? "on" : "off"));

        
        //Warp Buttons
        foreach (KeyValuePair<string,Warp> pair in SR2ESaveManager.data.warps.OrderBy(x => x.Key))
        {
            GameObject button = Instantiate(buttonTemplate, warpsContent);
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pair.Key;
            button.GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    AudioEUtil.PlaySound(MenuSound.Click);
                    Close();        
                    SR2EError error = pair.Value.WarpPlayerThere();
                    switch (error)
                    {
                        case SR2EError.TeleportablePlayerNull: SR2ELogManager.SendError(translation("cmd.error.teleportableplayernull")); break;
                        case SR2EError.SRCharacterControllerNull: SR2ELogManager.SendError(translation("cmd.error.srccnull")); break;
                    }
                }));
        }

        //Cheat Slots
        int i = -1;
        foreach (CheatMenuSlot slot in cheatSlots)
        {
            i++;
            slot.gameObject.SetActive(sceneContext.PlayerState.Ammo.Slots[i].IsUnlocked);
            slot.OnOpen();
        }
    }
    public override void OnCloseUIPressed()
    {
        if (MenuEUtil.isAnyPopUpOpen) return;
        
        Close();
    }

    private InputEvent inputDown;
    private InputEvent inputUp;
    public override void OnGameContext(GameContext gameContext)
    {
        inputDown = Get<InputEvent>("ItemDown");
        inputUp = Get<InputEvent>("ItemUp");
        var refScroll = refineryContent.parent.parent;
        if (!refScroll.HasComponent<ScrollByMenuKeys>())
        {
            var comp = refScroll.gameObject.AddComponent<ScrollByMenuKeys>();
            comp._scrollDownInput = inputDown;
            comp._scrollUpInput = inputUp;
            comp._scrollPerFrame = 9f;
        }
        var gadgetScroll = gadgetsContent.parent.parent;
        if (!gadgetScroll.HasComponent<ScrollByMenuKeys>())
        {
            var comp = gadgetScroll.gameObject.AddComponent<ScrollByMenuKeys>();
            comp._scrollDownInput = inputDown;
            comp._scrollUpInput = inputUp;
            comp._scrollPerFrame = 9f;
        }
    }

    protected override void OnLateAwake()
    {
        cheatButtonContent = transform.GetObjectRecursively<Transform>("CheatMenuCheatButtonsContentRec");
        refineryContent = transform.GetObjectRecursively<Transform>("CheatMenuRefineryContentRec");
        warpsContent = transform.GetObjectRecursively<Transform>("CheatMenuWarpsContentRec");
        gadgetsContent = transform.GetObjectRecursively<Transform>("CheatMenuGadgetContentRec");
        buttonTemplate = transform.GetObjectRecursively<GameObject>("CheatMenuTemplateButton");
        refineryEntryTemplate = transform.GetObjectRecursively<GameObject>("CheatMenuRefineryTemplateEntry");
        gadgetsEntryTemplate = transform.GetObjectRecursively<GameObject>("CheatMenuGadgetsTemplateEntry");
        
        CheatButtons();
        cheatSlots.Add(transform.GetObjectRecursively<GameObject>("CheatMenuStatsSlot1Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.GetObjectRecursively<GameObject>("CheatMenuStatsSlot2Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.GetObjectRecursively<GameObject>("CheatMenuStatsSlot3Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.GetObjectRecursively<GameObject>("CheatMenuStatsSlot4Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.GetObjectRecursively<GameObject>("CheatMenuStatsSlot5Rec").AddComponent<CheatMenuSlot>());
        cheatSlots.Add(transform.GetObjectRecursively<GameObject>("CheatMenuStatsSlot6Rec").AddComponent<CheatMenuSlot>());

        transform.GetObjectRecursively<GameObject>("CheatMenuStatsNewbucksRec").AddComponent<CheatMenuNewbucks>();
        
        var button1 = transform.GetObjectRecursively<Image>("CheatMenuMainSelectionButtonRec");
        button1.sprite = whitePillBg;
        button1.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        var button2 = transform.GetObjectRecursively<Image>("CheatMenuRefinerySelectionButtonRec");
        button2.sprite = whitePillBg;
        button2.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        var button3 = transform.GetObjectRecursively<Image>("CheatMenuGadgetsSelectionButtonRec");
        button3.sprite = whitePillBg;
        button3.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        var button4 = transform.GetObjectRecursively<Image>("CheatMenuSpawnSelectionButtonRec");
        button4.sprite = whitePillBg;
        button4.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.main");
        toTranslate.Add(button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.refinery");
        toTranslate.Add(button3.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.gadgets");
        toTranslate.Add(button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"cheatmenu.category.spawn");
        toTranslate.Add(transform.GetObjectRecursively<TextMeshProUGUI>("TitleTextRec"),"cheatmenu.title");
    }
    void CheatButtons()
    {
        if (EnableInfEnergy.HasFlag()) infEnergyButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infenergyoff"),
            () =>
        {
            AudioEUtil.PlaySound(MenuSound.Click);
            SR2ECommandManager.ExecuteByString("infenergy", true,true);
            infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        });
        if (EnableInfHealth.HasFlag()) infHealthButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infhealthoff"),
            () =>
        {
            AudioEUtil.PlaySound(MenuSound.Click);
            SR2ECommandManager.ExecuteByString("infhealth", true,true);
            infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
            });
        removeFogButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.removeFogoff"),
            () =>
            {
                AudioEUtil.PlaySound(MenuSound.Click);
                removeFog = !removeFog;
                removeFogButton.textInstance.text = translation("cheatmenu.cheatbuttons.removeFog" + (removeFog? "on" : "off"));
            });
        betterScreenshotButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.betterScreenshotoff"),
            () =>
            {
                AudioEUtil.PlaySound(MenuSound.Click);
                betterScreenshot = !betterScreenshot;
                betterScreenshotButton.textInstance.text = translation("cheatmenu.cheatbuttons.betterScreenshot" + (betterScreenshot? "on" : "off"));
            });
        noclipButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.noclipoff"),
            () =>
            {
                AudioEUtil.PlaySound(MenuSound.Click);
                SR2ECommandManager.ExecuteByString("noclip", true,true);
                noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (sceneContext.Camera.GetComponent<NoClipComponent>()!=null ? "on" : "off"));
            });
        refillButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.refillinv"), () =>
        {
            AudioEUtil.PlaySound(MenuSound.Click);
            SR2ECommandManager.ExecuteByString("refillinv", true,true);
        });
        
    }
}