using System;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI.UIStyling;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E.Menus;

public class SR2ERepoMenu : SR2EMenu
{
    //Check valid themes for all menus EVERYWHERE
    public new static MenuIdentifier GetMenuIdentifier() => new ("repomenu",SR2EMenuFont.SR2,SR2EMenuTheme.Default,"RepoMenu");
    public new static void PreAwake(GameObject obj) => obj.AddComponent<SR2ERepoMenu>();
    public override bool createCommands => true;
    public override bool inGameOnly => false;
    
    protected override void OnAwake()
    {
        SR2EEntryPoint.menus.Add(this, new Dictionary<string, object>()
        {
            {"requiredFeatures",new List<FeatureFlag>(){EnableRepoMenu}},
            {"openActions",new List<MenuActions> { MenuActions.PauseGame,MenuActions.HideMenus }},
            {"closeActions",new List<MenuActions> { MenuActions.UnPauseGame,MenuActions.UnHideMenus,MenuActions.EnableInput }},
        });
    }

    protected override void OnClose()
    {
        
    }
    protected override void OnOpen()
    {
        
    }
    
    protected override void OnLateAwake()
    {
        
        //var button1 = transform.getObjRec<Image>("ThemeMenuThemeSelectorSelectionButtonRec");
        //button1.sprite = whitePillBg;
        
        //toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"thememenu.category.selector");
        toTranslate.Add(transform.getObjRec<TextMeshProUGUI>("TitleTextRec"),"thememenu.title");
    }

    protected override void OnUpdate()
    {
        if (Key.Escape.OnKeyPressed())
            if(openPopUps.Count==0) 
                Close();
    }
}