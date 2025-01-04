using System;
using System.Linq;
using Il2CppTMPro;
using SR2E.Managers;
using SR2E.Menus;
using SR2E.Storage;

namespace SR2E;
/// <summary>
/// Abstract menu class
/// </summary>
[RegisterTypeInIl2Cpp(false)]
public class SR2EMenu : MonoBehaviour
{
    public static MenuIdentifier GetMenuIdentifier() => new ();
    public List<FeatureFlag> requiredFeatures => SR2EEntryPoint.menus[this][nameof(requiredFeatures)] as List<FeatureFlag>;
    public List<MenuActions> openActions => SR2EEntryPoint.menus[this][nameof(openActions)] as List<MenuActions>;
    public List<MenuActions> closeActions => SR2EEntryPoint.menus[this][nameof(closeActions)] as List<MenuActions>;
    protected virtual void OnClose() {}
    protected virtual void OnOpen() {}
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
        foreach (FeatureFlag featureFlag in requiredFeatures) if (!featureFlag.HasFlag()) return;
        if (!isOpen) return;
        menuBlock.SetActive(false);
        gameObject.SetActive(false);
        DoActions(closeActions);
        try { OnClose(); }catch (Exception e) { MelonLogger.Error(e); }
    }
    
    public new void Open()
    {
        foreach (FeatureFlag featureFlag in requiredFeatures) if (!featureFlag.HasFlag()) return;
        if (isAnyMenuOpen) return;
        if (SR2ESaveManager.WarpManager.warpTo != null) return;
        switch (SystemContext.Instance.SceneLoader.CurrentSceneGroup.name)
        {
            case "StandaloneStart":
            case "CompanyLogo":
            case "LoadScene":
                return;
        }
        menuBlock.SetActive(true);
        gameObject.SetActive(true);
        
        DoActions(openActions);
        try { OnOpen(); }catch (Exception e) { MelonLogger.Error(e); }
        foreach (var pair in toTranslate) pair.Key.SetText(translation(pair.Value));
    }
    
    public new void Toggle()
    {
        foreach (FeatureFlag featureFlag in requiredFeatures) if (!featureFlag.HasFlag()) return;
        if (isOpen) Close();
        else Open();
    }
    
    public bool isOpen { get {
        foreach (FeatureFlag featureFlag in requiredFeatures) if (!featureFlag.HasFlag()) return false;
            return gameObject.activeSelf; } }
    protected Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();
}

public enum MenuActions
{
    PauseGame, PauseGameFalse, UnPauseGame, UnPauseGameFalse, HideMenus, UnHideMenus, DisableInput, EnableInput
}