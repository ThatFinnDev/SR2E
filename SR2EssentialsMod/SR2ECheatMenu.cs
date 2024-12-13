using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppMonomiPark.SlimeRancher.UI.Refinery;
using Il2CppSystem.Linq;
using Il2CppTMPro;
using SR2E.Buttons;
using SR2E.Commands;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E;

public class SR2ECheatMenu
{
    internal static List<SR2ECheatMenuButton> cheatButtons = new List<SR2ECheatMenuButton>();
    internal static List<CheatMenuRefineryEntry> refineryEntries = new List<CheatMenuRefineryEntry>();
    internal static List<CheatMenuGadgetEntry> gadgetEntries = new List<CheatMenuGadgetEntry>();
    internal static List<CheatMenuSlot> cheatSlots = new List<CheatMenuSlot>();
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    internal static GameObject cheatMenuBlock;
    internal static Transform cheatButtonContent;
    internal static Transform refineryContent;
    internal static Transform gadgetsContent;
    internal static Transform warpsContent;
    internal static bool isOpen
    {
        get { return gameObject == null ? false : gameObject.activeSelf; }
    }
    /// <summary>
    /// Closes the cheat menu
    /// </summary>
    public static void Close()
    {
        if (Object.FindObjectsOfType<MapUI>().Length != 0) return;
        cheatMenuBlock.SetActive(false);
        gameObject.SetActive(false);
        gameObject.getObjRec<Button>("CheatMenuMainSelectionButtonRec").onClick.Invoke();

        SystemContext.Instance.SceneLoader.UnpauseGame();
    }


    /// <summary>
    /// Opens the cheat menu
    /// </summary>
    public static void Open()
    {
        if (SR2EConsole.isOpen) return;
        if (SR2EModMenu.isOpen) return;
        cheatMenuBlock.SetActive(true);
        gameObject.SetActive(true);

        try
        {
            PauseMenuRoot pauseMenuRoot = Object.FindObjectOfType<PauseMenuRoot>(); 
            pauseMenuRoot.Close();
            SystemContext.Instance.SceneLoader.TryPauseGame();
        }catch { }
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
        }


        noclipButton.textInstance.text = translation("cheatmenu.cheatbuttons.noclip" + (SceneContext.Instance.Camera.GetComponent<NoClipComponent>() == null ? "off" : "on"));
        infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
        infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));

        
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
            if (Key.Escape.kc().wasPressedThisFrame)
                Close();
        }
    }
    static GameObject buttonTemplate;
    static GameObject refineryEntryTemplate;
    static GameObject gadgetsEntryTemplate;
    internal static void Start()
    {
        cheatMenuBlock = parent.getObjRec<GameObject>("cheatMenuBlockRec");
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
        
        transform.getObjRec<Image>("CheatMenuMainSelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("CheatMenuRefinerySelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("CheatMenuGadgetsSelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("CheatMenuSpawnSelectionButtonRec").sprite = whitePillBg;

    }

    public static SR2ECheatMenuButton noclipButton;
    public static SR2ECheatMenuButton infEnergyButton;
    public static SR2ECheatMenuButton infHealthButton;
    static void CheatButtons()
    {
        infEnergyButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infenergyoff"),
            () =>
        {
            SR2EConsole.ExecuteByString("infenergy", true,true);
            infEnergyButton.textInstance.text = translation("cheatmenu.cheatbuttons.infenergy" + (InfiniteEnergyCommand.infEnergy? "on" : "off"));
        });
        infHealthButton = new SR2ECheatMenuButton(translation("cheatmenu.cheatbuttons.infhealthoff"),
            () =>
        {
            SR2EConsole.ExecuteByString("infhealth", true,true);
            infHealthButton.textInstance.text = translation("cheatmenu.cheatbuttons.infhealth" + (InfiniteHealthCommand.infHealth? "on" : "off"));
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