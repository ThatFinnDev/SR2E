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

public class SR2ECheatMenu : SR2EMenu
{
    public new static MenuIdentifier GetMenuIdentifier() => new MenuIdentifier(true,"cheatmenu",SR2EMenuTheme.Default,"CheatMenu");
    public new static void PreAwake(GameObject obj) => obj.AddComponent<SR2ECheatMenu>();
    protected override void OnAwake()
    {
        SR2EEntryPoint.menus.Add(this, new Dictionary<string, object>()
        {
            {"requiredFeatures",new List<FeatureFlag>(){EnableCheatMenu}},
            {"openActions",new List<MenuActions> { MenuActions.PauseGame,MenuActions.HideMenus }},
            {"closeActions",new List<MenuActions> { MenuActions.UnPauseGame,MenuActions.UnHideMenus,MenuActions.EnableInput }},
        });
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
    SR2ECheatMenuButton noclipButton;
    SR2ECheatMenuButton infEnergyButton;
    SR2ECheatMenuButton infHealthButton;
    SR2ECheatMenuButton removeFogButton;
    SR2ECheatMenuButton betterScreenshotButton;
    internal static bool removeFog = false;
    internal static bool betterScreenshot = false;
    
    protected override void OnClose()
    {
        gameObject.getObjRec<Button>("CheatMenuMainSelectionButtonRec").onClick.Invoke();
        refineryContent.DestroyAllChildren();
        gadgetsContent.DestroyAllChildren();
        cheatButtonContent.DestroyAllChildren();
        warpsContent.DestroyAllChildren();
    }
    
    protected override void OnOpen()
    {
        //Refinery
        List<IdentifiableType> refineryItems = SceneContext.Instance.GadgetDirector._refineryTypeGroup.GetAllMembers().ToArray().ToList();
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
        
        List<IdentifiableType> gadgetItems = SceneContext.Instance.GadgetDirector._gadgetsGroup.GetAllMembers().ToArray().ToList();
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


        noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (SceneContext.Instance.Camera.GetComponent<NoClipComponent>() == null ? "off" : "on"));
        if (EnableInfHealth.HasFlag()) infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
        if (EnableInfEnergy.HasFlag()) infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        removeFogButton.textInstance.text = translation("cheatmenu.cheatbuttons.removeFog" + (removeFog? "on" : "off"));
        betterScreenshotButton.textInstance.text = translation("cheatmenu.cheatbuttons.betterScreenshot" + (betterScreenshot? "on" : "off"));

        
        //Warp Buttons
        foreach (KeyValuePair<string,SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps.OrderBy(x => x.Key))
        {
            GameObject button = Instantiate(buttonTemplate, warpsContent);
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
    }

    protected override void OnUpdate()
    {
       if (Key.Escape.OnKeyPressed())
           Close();
        
    }
    protected override void OnLateAwake()
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
    void CheatButtons()
    {
        if (EnableInfEnergy.HasFlag()) infEnergyButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infenergyoff"),
            () =>
        {
            SR2ECommandManager.ExecuteByString("infenergy", true,true);
            infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        });
        if (EnableInfHealth.HasFlag()) infHealthButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infhealthoff"),
            () =>
        {
            SR2ECommandManager.ExecuteByString("infhealth", true,true);
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
                SR2ECommandManager.ExecuteByString("noclip", true,true);
                noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (SceneContext.Instance.Camera.GetComponent<NoClipComponent>()!=null ? "on" : "off"));
            });
        new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.refillinv"), () => { SR2ECommandManager.ExecuteByString("refillinv", true,true); });
        
    }
}