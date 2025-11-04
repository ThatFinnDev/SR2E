using System;
using System.Reflection;
using Il2CppInterop.Runtime.Attributes;
using Il2CppTMPro;
using SR2E.Commands;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Enums.Sounds;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E;

/// <summary>
/// Abstract menu class
/// </summary>
[RegisterTypeInIl2Cpp(false)]
public abstract class SR2EMenu : MonoBehaviour
{
    private bool changedOpenState = false;


    public static MenuIdentifier GetMenuIdentifier() => new();

    //SR2EMenu doesnt work for whatever reason
    private SR2EMenu _menuToOpenOnClose;
    public virtual bool createCommands => false;

    public virtual bool inGameOnly => false;

    //private List<FeatureFlag> requiredFeatures => SR2EEntryPoint.menus[this][nameof(requiredFeatures)] as List<FeatureFlag>;
    //public List<MenuActions> openActions => SR2EEntryPoint.menus[this][nameof(openActions)] as List<MenuActions>;
    //public List<MenuActions> closeActions => SR2EEntryPoint.menus[this][nameof(closeActions)] as List<MenuActions>;
    protected virtual void OnClose()
    {
    }

    protected virtual void OnOpen()
    {
    }
    public virtual void OnCloseUIPressed()
    {
    }
    public virtual void ApplyFont(TMP_FontAsset font)
    {
        foreach (var text in gameObject.GetAllChildrenOfType<TMP_Text>())
            text.font = font;
    }

    public void Awake()
    {
        try {OnAwake();}
        catch (Exception e) { MelonLogger.Error(e); }
        SR2EEntryPoint.menus.TryAdd(this, new Dictionary<string, object>()
        {
            { "requiredFeatures", new List<FeatureFlag>() },
            { "openActions", new List<MenuActions> { MenuActions.PauseGame, MenuActions.HideMenus } },
            {
                "closeActions",
                new List<MenuActions> { MenuActions.UnPauseGame, MenuActions.UnHideMenus, MenuActions.EnableInput }
            },
        });
        if (createCommands)
        {
            bool error = false;
            try
            {
                MenuIdentifier identifier = this.GetMenuIdentifier();
                if (!string.IsNullOrEmpty(identifier.saveKey))
                {
                    try { SR2ECommandManager.RegisterCommand(new MenuVisibilityCommands.OpenCommand(identifier, this, inGameOnly)); }
                    catch (Exception e) { error=true; MelonLogger.Error(e); }

                    try { SR2ECommandManager.RegisterCommand(new MenuVisibilityCommands.ToggleCommand(identifier, this, inGameOnly)); }
                    catch (Exception e) { error=true; MelonLogger.Error(e); }

                    try { SR2ECommandManager.RegisterCommand(new MenuVisibilityCommands.CloseCommand(identifier, this, inGameOnly)); } 
                    catch (Exception e) { error=true; MelonLogger.Error(e); }
                }
            }
            catch
            {
                error = true;
            }
            if(error) MelonLogger.Error("There was an error creating menu commands");
        }

        if (MenuEUtil.menuBlock == null)
            MenuEUtil.menuBlock = transform.parent.GetObjectRecursively<GameObject>("blockRec");
        if (MenuEUtil.popUpBlock == null)
            MenuEUtil.popUpBlock = transform.parent.GetObjectRecursively<Transform>("blockPopUpRec");
        try {OnLateAwake();}
        catch (Exception e) { MelonLogger.Error(e); }
    }

    protected virtual void OnAwake()
    {
    }

    protected virtual void OnLateAwake()
    {
    }

    protected virtual void OnStart()
    {
    }

    protected void Start()
    {
        try {OnStart();}
        catch (Exception e) { MelonLogger.Error(e); }
        gameObject.SetActive(false);
    }

    protected internal void AlwaysUpdate()
    {
        changedOpenState = false;
        try {OnAlwaysUpdate();}
        catch (Exception e) { MelonLogger.Error(e); }
    }

    protected virtual void OnAlwaysUpdate()
    {
    }

    protected void Update()
    {
        changedOpenState = false;
        try {OnUpdate();}
        catch (Exception e) { MelonLogger.Error(e); }
    }

    protected virtual void OnUpdate()
    {
    }

    private bool _closing;
    public new void Close()
    {
        _closing = true;
        if (changedOpenState) return;
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>)
            if (!featureFlag.HasFlag())
                return;
        if (!isOpen) return;
        MenuEUtil.menuBlock.SetActive(false);
        gameObject.SetActive(false);
        changedOpenState = true;
        foreach (SR2EPopUp popUp in MenuEUtil.openPopUps) popUp.Close();
        MenuEUtil.DoMenuActions(SR2EEntryPoint.menus[this]["closeActions"] as List<MenuActions>);
        try
        {
            OnClose();
        }
        catch (Exception e)
        {
            MelonLogger.Error(e);
        }

        _closing = false;
        if (_menuToOpenOnClose != null)
            ExecuteInTicks((Action)(() =>
            {
                _menuToOpenOnClose.TryCast<SR2EMenu>().Open();
                _menuToOpenOnClose = null;
            }), 2);
        AudioEUtil.PlaySound(MenuSound.CloseMenu);
    }

    [HideFromIl2Cpp]
    protected Action SelectCategorySound
    {
        get
        {
            return (Action)(() =>
            {
                if(_closing) return;
                AudioEUtil.PlaySound(MenuSound.SelectCategory);
            });
        }
    }
    //SR2EMenu doesnt work for whatever reason
    public void OpenC(MonoBehaviour menuToOpenOnClose)
    {
        if (!(menuToOpenOnClose is SR2EMenu)) return;
        _menuToOpenOnClose = menuToOpenOnClose.TryCast<SR2EMenu>();
        Open();
    }

    public new void Open()
    {
        if (changedOpenState) return;
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>) if (!featureFlag.HasFlag()) return;
        if (MenuEUtil.isAnyMenuOpen) return;
        if(inGameOnly) if (!inGame) return;
        if (SR2EWarpManager.warpTo != null) return;
        foreach (var pair in SR2EEntryPoint.menus)
            if(pair.Key!=this) pair.Key._menuToOpenOnClose = null;
        
        switch (systemContext.SceneLoader.CurrentSceneGroup.name)
        {
            case "StandaloneStart":
            case "CompanyLogo":
            case "LoadScene":
                return;
        }
        MenuEUtil.menuBlock.SetActive(true);
        gameObject.SetActive(true);
        changedOpenState = true;
        ExecuteInTicks((Action)(() => { gameObject.SetActive(true);}), 1);
        MenuEUtil.DoMenuActions(SR2EEntryPoint.menus[this]["openActions"] as List<MenuActions>);
        try { OnOpen(); }catch (Exception e) { MelonLogger.Error(e); }
        foreach (var pair in toTranslate) pair.Key.SetText(translation(pair.Value));
        AudioEUtil.PlaySound(MenuSound.OpenMenu);
    }
    
    public new void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
    
    public bool isOpen { get {
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>) if (!featureFlag.HasFlag()) return false;
            return gameObject.activeSelf; } }
    protected Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();


    protected Sprite whitePillBg => MenuEUtil.whitePillBg; 
    protected Texture2D whitePillBgTex => MenuEUtil.whitePillBgTex;
    
    
    /// <summary>
    /// Gets executed once GameContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void OnGameContext(GameContext gameContext) { }
}

