using System.Reflection;
using Il2CppTMPro;
using SR2E.Commands;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E;
/// <summary>
/// Abstract menu class
/// </summary>
[RegisterTypeInIl2Cpp(false)]
public abstract class SR2EMenu : MonoBehaviour
{
    public static MenuIdentifier GetMenuIdentifier() => new ();
    public virtual bool createCommands => false;
    public virtual bool inGameOnly => false;
    //private List<FeatureFlag> requiredFeatures => SR2EEntryPoint.menus[this][nameof(requiredFeatures)] as List<FeatureFlag>;
    //public List<MenuActions> openActions => SR2EEntryPoint.menus[this][nameof(openActions)] as List<MenuActions>;
    //public List<MenuActions> closeActions => SR2EEntryPoint.menus[this][nameof(closeActions)] as List<MenuActions>;
    protected virtual void OnClose() {}
    protected virtual void OnOpen() {}

    public virtual void ApplyFont(TMP_FontAsset font)
    {
        foreach (var text in gameObject.getAllChildrenOfType<TMP_Text>())
            text.font = font;
    }
    public static void PreAwake(GameObject obj) {}
    public void Awake()
    {
        OnAwake();
        SR2EEntryPoint.menus.TryAdd(this, new Dictionary<string, object>()
        {
            {"requiredFeatures",new List<FeatureFlag>()},
            {"openActions",new List<MenuActions> { MenuActions.PauseGame,MenuActions.HideMenus }},
            {"closeActions",new List<MenuActions> { MenuActions.UnPauseGame,MenuActions.UnHideMenus,MenuActions.EnableInput }},
        });
        if (createCommands)
        {
            //Requires Reflection to get overriden one
            try
            {
                MenuIdentifier identifier = this.GetIdentifierViaReflection();
                if (!string.IsNullOrEmpty(identifier.saveKey))
                {
                    try { SR2ECommandManager.RegisterCommand(new MenuVisibilityCommands.OpenCommand(identifier,this,inGameOnly)); } catch (Exception e) { MelonLogger.Error(e); }
                    try { SR2ECommandManager.RegisterCommand(new MenuVisibilityCommands.ToggleCommand(identifier,this,inGameOnly)); } catch (Exception e) { MelonLogger.Error(e); }
                    try { SR2ECommandManager.RegisterCommand(new MenuVisibilityCommands.CloseCommand(identifier,this,inGameOnly)); } catch (Exception e) { MelonLogger.Error(e); }
                }
            }
            catch {MelonLogger.Error("There was an error creating menu commands");}
        }
        OnLateAwake();
    } 
    protected virtual void OnAwake() {}
    protected virtual void OnLateAwake() {}
    protected virtual void OnStart() {}
    protected void Start(){
        OnStart();
        gameObject.SetActive(false);
    } 
    protected void Update() => OnUpdate(); protected virtual void OnUpdate() {}

    

    public new void Close()
    {
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>) if (!featureFlag.HasFlag()) return;
        if (!isOpen) return;
        menuBlock.SetActive(false);
        gameObject.SetActive(false);
        DoActions(SR2EEntryPoint.menus[this]["closeActions"] as List<MenuActions>);
        try { OnClose(); }catch (Exception e) { MelonLogger.Error(e); }
    }
    
    public new void Open()
    {
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>) if (!featureFlag.HasFlag()) return;
        if (isAnyMenuOpen) return;
        if(inGameOnly) if (!inGame) return;
        if (SR2EWarpManager.warpTo != null) return;
        switch (SystemContext.Instance.SceneLoader.CurrentSceneGroup.name)
        {
            case "StandaloneStart":
            case "CompanyLogo":
            case "LoadScene":
                return;
        }
        menuBlock.SetActive(true);
        gameObject.SetActive(true);
        
        DoActions(SR2EEntryPoint.menus[this]["openActions"] as List<MenuActions>);
        try { OnOpen(); }catch (Exception e) { MelonLogger.Error(e); }
        foreach (var pair in toTranslate) pair.Key.SetText(translation(pair.Value));
    }
    
    public new void Toggle()
    {
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>) if (!featureFlag.HasFlag()) return;
        if (isOpen) Close();
        else Open();
    }
    
    public bool isOpen { get {
        foreach (FeatureFlag featureFlag in SR2EEntryPoint.menus[this]["requiredFeatures"] as List<FeatureFlag>) if (!featureFlag.HasFlag()) return false;
            return gameObject.activeSelf; } }
    protected Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();
}

