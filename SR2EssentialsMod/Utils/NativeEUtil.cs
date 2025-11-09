using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.Components;
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
                var ui = GetAnyInScene<MainMenuLandingRootUI>();
                ui.gameObject.SetActive(false);
                ui.enabled = false;
                ui.Close(true, null);
            }  
            catch (Exception e) { MelonLogger.Error(e); }
        }

        if (inGame)
        {
            try
            {
                GetAnyInScene<PauseMenuRoot>().HideUI();
            } catch { }
        }
    }

    public static void TryPauseAndHide()
    {
        if (inGame&&Object.FindObjectOfType<PauseMenuRoot>())
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
        if (SR2EEntryPoint.mainMenuLoaded)
            Time.timeScale = 0;
        
        try
        {
            systemContext.SceneLoader.TryPauseGame();
        } catch { }

        if (usePauseMenu)
            try
            {
                sceneContext.PauseMenuDirector.PauseGame();
            } catch { }
    }

    public static void TryUnPauseGame(bool usePauseMenu = true)
    {

        if (SR2EEntryPoint.mainMenuLoaded)
            Time.timeScale = 1;
        
        try
        {
            systemContext.SceneLoader.UnpauseGame();
        } catch { }

        if (usePauseMenu)
            try
            {
                sceneContext.PauseMenuDirector.UnPauseGame();
            } catch { }
    }

    public static void TryUnHideMenus()
    {
        try
        {
            if (SR2EEntryPoint.mainMenuLoaded)
            {
                try
                {
                    foreach (UIDisplayContainer container in GetAllInScene<UIDisplayContainer>())
                        if (container.TargetContainer.name == "MainMenuRoot" && container.name == "MainMenuRoot")
                        {
                            try
                            {

                                container.OnEnable();
                                break;
                            } catch {}
                        }
                } catch {}
            }
            if (inGame) HudUI.Instance.transform.GetChild(0).gameObject.SetActive(true);
        }
        catch { }
    }

    internal static float _CustomTimeScale = 1f;

    public static float CustomTimeScale
    {
        get { return _CustomTimeScale; }
        set
        {
            _CustomTimeScale = value;
            if (value < 0.01f) _CustomTimeScale = 0.01f;
            if (value > 2000f) _CustomTimeScale = 2000f;
            SR2EEntryPoint.CheckForTime();
        }
    }

    public static void TryDisableSR2Input()
    {
        try
        {
            gameContext.InputDirector._paused.Map.Disable();
            gameContext.InputDirector._mainGame.Map.Disable();
        } catch { }
    }

    public static void TryEnableSR2Input()
    {
        try
        {
            gameContext.InputDirector._paused.Map.Enable();
            gameContext.InputDirector._mainGame.Map.Enable();
        } catch { }
    }
}