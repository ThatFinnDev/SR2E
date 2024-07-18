using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using SR2E.Buttons;
using UnityEngine.UI;

namespace SR2E;

public class SR2ECheatMenu
{
    internal static List<SR2ECheatMenuButton> cheatButtons = new List<SR2ECheatMenuButton>();
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    internal static GameObject cheatMenuBlock;
    internal static Transform cheatButtonContent;
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
        }
        catch
        {
        }

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

    }

    /// <summary>
    /// Toggles the mod menu
    /// </summary>
    public static void Toggle()
    {

        if (isOpen) Close();
        else Open();
    }

    static GameObject buttonTemplate;
    internal static void Start()
    {
        cheatMenuBlock = parent.getObjRec<GameObject>("cheatMenuBlockRec");
        cheatButtonContent = transform.getObjRec<Transform>("CheatMenuCheatButtonsContentRec");
        warpsContent = transform.getObjRec<Transform>("CheatMenuWarpsContentRec");
        buttonTemplate = transform.getObjRec<GameObject>("CheatMenuTemplateButton");

        new SR2ECheatMenuButton("DecTime", 0, null);
        new SR2ECheatMenuButton("IncTime", 0, null);
        new SR2ECheatMenuButton("InfEnergy [Off]", 0, null);
        new SR2ECheatMenuButton("InfHealth [Off]", 0, null);
        new SR2ECheatMenuButton("NoClip", 0, null);
        new SR2ECheatMenuButton("RefillItems", 0, null);

        
        transform.getObjRec<Image>("CheatMenuMainSelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("CheatMenuRefinerySelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("CheatMenuGadgetsSelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("CheatMenuSpawnSelectionButtonRec").sprite = whitePillBg;

    }
}