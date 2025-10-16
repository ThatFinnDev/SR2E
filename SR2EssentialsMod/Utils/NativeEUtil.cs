using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.InputSystem.UI;

namespace SR2E.Utils;

public static class NativeEUtil
{
    public static void TryHideMenus()
    {
        if (SR2EEntryPoint.mainMenuLoaded)
        {
            try
            {
                var ui = Object.FindObjectOfType<MainMenuLandingRootUI>();
                ui.gameObject.SetActive(false);
                ui.enabled = false;
                ui.Close(true, null);
            } catch { }
        }

        if (inGame)
        {
            try
            {
                Object.FindObjectOfType<PauseMenuRoot>().Close();
            } catch { }
        }
    }

    public static void TryPauseAndHide()
    {
        if (Object.FindObjectOfType<PauseMenuRoot>())
        {
            TryHideMenus();
            TryPauseGame(false);
            if (inGame) HudUI.Instance.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            TryPauseGame();
            TryHideMenus();
        }
    }

    public static void TryPauseGame(bool usePauseMenu = true)
    {
        try
        {
            SystemContext.Instance.SceneLoader.TryPauseGame();
        } catch { }

        if (usePauseMenu)
            try
            {
                Object.FindObjectOfType<PauseMenuDirector>().PauseGame();
            } catch { }
    }

    public static void TryUnPauseGame(bool usePauseMenu = true, bool usePauseMenuElse = true)
    {
        try
        {
            SystemContext.Instance.SceneLoader.UnpauseGame();
        } catch { }

        if (usePauseMenu)
            try
            {
                Object.FindObjectOfType<PauseMenuDirector>().UnPauseGame();
            } catch { }
        else if (usePauseMenuElse)
            try
            {
                if (Object.FindObjectOfType<PauseMenuRoot>() != null)
                    Object.FindObjectOfType<PauseMenuDirector>().PauseGame();
            } catch { }
    }

    public static void TryUnHideMenus()
    {
        if (SR2EEntryPoint.mainMenuLoaded)
        {
            foreach (UIPrefabLoader loader in Object.FindObjectsOfType<UIPrefabLoader>())
                if (loader.gameObject.name == "UIActivator" && loader.uiPrefab.name == "MainMenu" &&
                    loader.parentTransform.name == "MainMenuRoot")
                {
                    loader.Start();
                    break;
                }
        }

        if (inGame) HudUI.Instance.transform.GetChild(0).gameObject.SetActive(true);
    }

    internal static float _CustomTimeScale = 1f;

    public static float CustomTimeScale
    {
        get { return _CustomTimeScale; }
        set
        {
            _CustomTimeScale = value;
            SR2EEntryPoint.CheckForTime();
        }
    }

    public static void TryDisableSR2Input()
    {
        Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Disable();
    }

    public static void TryEnableSR2Input()
    {
        Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Enable();
    }
}