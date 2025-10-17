using System;
using System.Collections;
using System.Threading;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Managers;
using SR2E.Popups;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E.Menus;

public class SR2ERepoMenu : SR2EMenu
{
    //Check valid themes for all menus EVERYWHERE
    public new static MenuIdentifier GetMenuIdentifier() => new ("repomenu",SR2EMenuFont.SR2,SR2EMenuTheme.Default,"RepoMenu");
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
        gameObject.GetObjectRecursively<Button>("RepoMenuMainSelectionButtonRec").onClick.Invoke();
        Transform modContent = transform.GetObjectRecursively<Transform>("RepoMenuMainContentRec");
        Transform repoContent = transform.GetObjectRecursively<Transform>("RepoMenuRepoContentRec");
        for (int i = 0; i < modContent.childCount; i++)
            Destroy(modContent.GetChild(i).gameObject);
        for (int i = 0; i < repoContent.childCount; i++)
            Destroy(repoContent.GetChild(i).gameObject);
    }
    Transform repoPanel;
    Transform modPanel;
    
    protected override void OnOpen()
    {
        GameObject buttonPrefab = transform.GetObjectRecursively<GameObject>("RepoMenuTemplateButton");
        Transform modContent = transform.GetObjectRecursively<Transform>("RepoMenuMainContentRec");
        Transform repoContent = transform.GetObjectRecursively<Transform>("RepoMenuRepoContentRec");
        repoPanel = transform.GetObjectRecursively<Transform>("RepoViewPanelRec");
        modPanel = transform.GetObjectRecursively<Transform>("ModViewPanelRec");
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
                        var name = transform.GetObjectRecursively<TextMeshProUGUI>("RepoViewNameTextRec");
                        var desc = transform.GetObjectRecursively<TextMeshProUGUI>("RepoViewDescriptionTextRec");
                        
                        if (!string.IsNullOrWhiteSpace(repo.Value.header_url))
                        { 
                            HttpEUtil.DownloadTexture2DIntoImageAsync(repo.Value.header_url,
                                transform.GetObjectRecursively<Image>("RepoViewHeaderImageRec"));
                        }
                        if(string.IsNullOrWhiteSpace(repo.Value.name)) name.gameObject.SetActive(false);
                        else {name.gameObject.SetActive(true); name.SetText(repo.Value.name);}
                        
                        
                        if(string.IsNullOrWhiteSpace(repo.Value.description)) desc.gameObject.SetActive(false);
                        else {desc.gameObject.SetActive(true); desc.SetText("Description: "+repo.Value.description);}
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
                        var name = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewNameTextRec");
                        var author = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewAuthorTextRec");
                        var coauthors = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewCoAuthorTextRec");
                        var desc = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewDescriptionTextRec");
                        var company = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewCompanyTextRec");
                        var trademark = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewTrademarkTextRec");
                        var team = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewTeamTextRec");
                        var copyright = transform.GetObjectRecursively<TextMeshProUGUI>("ModViewCopyrightTextRec");


                        if (!string.IsNullOrWhiteSpace(mod.header_url))
                        {
                            HttpEUtil.DownloadTexture2DIntoImageAsync(mod.header_url, transform.GetObjectRecursively<Image>("ModViewHeaderImageRec"));
                        }

                        if (!string.IsNullOrWhiteSpace(mod.icon_url))
                        {
                            HttpEUtil.DownloadTexture2DIntoImageAsync(mod.icon_url, transform.GetObjectRecursively<Image>("ModViewIconImageRec"));
                        }
                        
                        if(string.IsNullOrWhiteSpace(mod.name)) name.gameObject.SetActive(false);
                        else {name.gameObject.SetActive(true); name.SetText(mod.name);}
                        
                        if(string.IsNullOrWhiteSpace(mod.author)) author.gameObject.SetActive(false);
                        else {author.gameObject.SetActive(true); author.SetText("Author: "+mod.author);}
                        
                        if(string.IsNullOrWhiteSpace(mod.coauthors)) coauthors.gameObject.SetActive(false);
                        else {coauthors.gameObject.SetActive(true); coauthors.SetText("Co-Authors: "+mod.coauthors);}
                        
                        if(string.IsNullOrWhiteSpace(mod.description)) desc.gameObject.SetActive(false);
                        else {desc.gameObject.SetActive(true); desc.SetText("Description: "+mod.description);}
                        
                        if(string.IsNullOrWhiteSpace(mod.company)) company.gameObject.SetActive(false);
                        else {company.gameObject.SetActive(true); company.SetText("Company: "+mod.company);}
                        
                        if(string.IsNullOrWhiteSpace(mod.trademark)) trademark.gameObject.SetActive(false);
                        else {trademark.gameObject.SetActive(true); trademark.SetText("Trademark: "+mod.trademark);}
                        
                        if(string.IsNullOrWhiteSpace(mod.team)) team.gameObject.SetActive(false);
                        else {team.gameObject.SetActive(true); team.SetText("Team: "+mod.team);}
                        
                        if(string.IsNullOrWhiteSpace(mod.copyright)) copyright.gameObject.SetActive(false);
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
        toTranslate.Add(transform.GetObjectRecursively<TextMeshProUGUI>("TitleTextRec"),"repomenu.title");
    }

    protected override void OnUpdate()
    {
        if (Key.Escape.OnKeyPressed())
            if(MenuEUtil.openPopUps.Count==0)
            {
                if(repoPanel.gameObject.activeSelf)
                    repoPanel.gameObject.SetActive(false);
                else if(modPanel.gameObject.activeSelf)
                    modPanel.gameObject.SetActive(false);
                else Close();
            }
    }
}