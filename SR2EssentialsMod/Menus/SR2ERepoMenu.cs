using System;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI.UIStyling;
using Il2CppTMPro;
using Newtonsoft.Json;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Managers;
using SR2E.Popups;
using SR2E.Repos;
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
        gameObject.getObjRec<Button>("RepoMenuMainSelectionButtonRec").onClick.Invoke();
        Transform modContent = transform.getObjRec<Transform>("RepoMenuMainContentRec");
        for (int i = 0; i < modContent.childCount; i++)
            Object.Destroy(modContent.GetChild(i).gameObject);
    }
    protected override void OnOpen()
    {
        GameObject buttonPrefab = transform.getObjRec<GameObject>("RepoMenuTemplateButton");
        Transform modContent = transform.getObjRec<Transform>("RepoMenuMainContentRec");
        Transform repoContent = transform.getObjRec<Transform>("RepoMenuRepoContentRec");
        foreach (var repo in SR2ERepoManager.repos)
        {
            if (repo.Value == null)
            {
                continue;
            }
            try
            {
                GameObject obj = Instantiate(buttonPrefab, repoContent);
                Button b = obj.GetComponent<Button>();
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = repo.Value.name;
                obj.SetActive(true);
                    
                b.onClick.AddListener((Action)(() =>
                {
                    //open repo infos
                    SR2ETextViewer.Open(JsonConvert.SerializeObject(repo));
                }));
            }
            catch {}
            foreach (var mod in repo.Value.mods)
            {
                try
                {
                    GameObject obj = Instantiate(buttonPrefab, modContent);
                    Button b = obj.GetComponent<Button>();
                    b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mod.name;
                    obj.SetActive(true);
                    
                    b.onClick.AddListener((Action)(() =>
                    {
                        //open repo infos
                        SR2ETextViewer.Open(JsonConvert.SerializeObject(mod));
                    }));
                }
                catch {}

            }
        }
        
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