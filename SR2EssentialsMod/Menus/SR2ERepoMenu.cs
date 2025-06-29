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
        Transform repoContent = transform.getObjRec<Transform>("RepoMenuRepoContentRec");
        for (int i = 0; i < modContent.childCount; i++)
            Destroy(modContent.GetChild(i).gameObject);
        for (int i = 0; i < repoContent.childCount; i++)
            Destroy(repoContent.GetChild(i).gameObject);
    }
    Transform repoPanel;
    Transform modPanel;
    protected override void OnOpen()
    {
        GameObject buttonPrefab = transform.getObjRec<GameObject>("RepoMenuTemplateButton");
        Transform modContent = transform.getObjRec<Transform>("RepoMenuMainContentRec");
        Transform repoContent = transform.getObjRec<Transform>("RepoMenuRepoContentRec");
        repoPanel = transform.getObjRec<Transform>("RepoViewPanelRec");
        modPanel = transform.getObjRec<Transform>("ModViewPanelRec");
        foreach (var repo in SR2ERepoManager.repos)
        {
            if (repo.Value == null)
            {
                GameObject obj = Instantiate(buttonPrefab, repoContent);
                Button b = obj.GetComponent<Button>();
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "BROKEN: "+repo.Key;
                obj.SetActive(true);
                ColorBlock colorBlock = b.colors;colorBlock.normalColor = new Color(0.5f, 0.5f, 0.5f, 1);
                colorBlock.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1); 
                colorBlock.pressedColor = new Color(0.3f, 0.3f, 0.3f, 1); 
                colorBlock.selectedColor = new Color(0.6f, 0.6f, 0.6f, 1); 
                b.colors = colorBlock;
                    
                b.onClick.AddListener((Action)(() =>
                {
                    //open repo infos
                    SR2ETextViewer.Open("There was an error fetching the repo!");
                }));
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
                    repoPanel.gameObject.SetActive(true);
                }));
            }
            catch {}
            foreach (var mod in repo.Value.mods)
            {
                if (mod == null) return;
                try
                {
                    GameObject obj = Instantiate(buttonPrefab, modContent);
                    Button b = obj.GetComponent<Button>();
                    b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mod.name;
                    obj.SetActive(true);
                    
                    b.onClick.AddListener((Action)(() =>
                    {
                        modPanel.gameObject.SetActive(true);
                        var name = transform.getObjRec<TextMeshProUGUI>("ModViewNameTextRec");
                        var author = transform.getObjRec<TextMeshProUGUI>("ModViewAuthorTextRec");
                        var coauthor = transform.getObjRec<TextMeshProUGUI>("ModViewCoAuthorTextRec");
                        var desc = transform.getObjRec<TextMeshProUGUI>("ModViewDescriptionTextRec");
                        var company = transform.getObjRec<TextMeshProUGUI>("ModViewCompanyTextRec");
                        var trademark = transform.getObjRec<TextMeshProUGUI>("ModViewTrademarkTextRec");
                        var team = transform.getObjRec<TextMeshProUGUI>("ModViewTeamTextRec");
                        var copyright = transform.getObjRec<TextMeshProUGUI>("ModViewCopyrightTextRec");
                        
                        if(String.IsNullOrWhiteSpace(mod.name)) name.gameObject.SetActive(false);
                        else {name.gameObject.SetActive(true); name.SetText(mod.name);}
                        
                        if(String.IsNullOrWhiteSpace(mod.author)) author.gameObject.SetActive(false);
                        else {author.gameObject.SetActive(true); author.SetText("Author: "+mod.author);}
                        
                        if(String.IsNullOrWhiteSpace(mod.coauthor)) coauthor.gameObject.SetActive(false);
                        else {coauthor.gameObject.SetActive(true); coauthor.SetText("Co-Author: "+mod.coauthor);}
                        
                        if(String.IsNullOrWhiteSpace(mod.description)) desc.gameObject.SetActive(false);
                        else {desc.gameObject.SetActive(true); desc.SetText("Description: "+mod.description);}
                        
                        if(String.IsNullOrWhiteSpace(mod.company)) company.gameObject.SetActive(false);
                        else {company.gameObject.SetActive(true); company.SetText("Company: "+mod.company);}
                        
                        if(String.IsNullOrWhiteSpace(mod.trademark)) trademark.gameObject.SetActive(false);
                        else {trademark.gameObject.SetActive(true); trademark.SetText("Trademark: "+mod.trademark);}
                        
                        if(String.IsNullOrWhiteSpace(mod.team)) team.gameObject.SetActive(false);
                        else {team.gameObject.SetActive(true); team.SetText("Team: "+mod.team);}
                        
                        if(String.IsNullOrWhiteSpace(mod.copyright)) copyright.gameObject.SetActive(false);
                        else {copyright.gameObject.SetActive(true); copyright.SetText("Copyright: "+mod.copyright);}
                            
                            
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
        toTranslate.Add(transform.getObjRec<TextMeshProUGUI>("TitleTextRec"),"repomenu.title");
    }

    protected override void OnUpdate()
    {
        if (Key.Escape.OnKeyPressed())
            if(openPopUps.Count==0)
            {
                if(repoPanel.gameObject.activeSelf)
                    repoPanel.gameObject.SetActive(false);
                else if(modPanel.gameObject.activeSelf)
                    modPanel.gameObject.SetActive(false);
                else Close();
            }
    }
}