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
    public new static MenuIdentifier GetMenuIdentifier() => new ("repomenu",SR2EMenuFont.SR2,SR2EMenuTheme.SR2E,"RepoMenu");
    public override bool createCommands => true;
    public override bool inGameOnly => false;
    
    protected override void OnAwake()
    {
        requiredFeatures = new List<FeatureFlag>() { EnableRepoMenu }.ToArray();
        openActions = new List<MenuActions> { MenuActions.PauseGame, MenuActions.HideMenus }.ToArray();
        closeActions = new List<MenuActions> { MenuActions.UnPauseGame, MenuActions.UnHideMenus, MenuActions.EnableInput }.ToArray();
    }


    protected override void OnClose()
    {
        gameObject.GetObjectRecursively<Button>("RepoMenuBrowseSelectionButtonRec").onClick.Invoke();
        transform.GetObjectRecursively<Transform>("RepoMenuBrowseContentRec").DestroyAllChildren();
        transform.GetObjectRecursively<Transform>("RepoMenuRepoContentRec").DestroyAllChildren();
    }
    Transform repoPanel;
    Transform modPanel;
    
    protected override void OnOpen()
    {
        gameObject.GetObjectRecursively<Button>("RepoMenuBrowseSelectionButtonRec").onClick.Invoke();
        
    }
    
    protected override void OnLateAwake()
    {
        
        var button1 = transform.GetObjectRecursively<Image>("RepoMenuBrowseSelectionButtonRec");
        button1.sprite = whitePillBg;
        button1.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        button1.GetComponent<Button>().onClick.AddListener((System.Action)(() => OnBrowseTab()));
        var button2 = transform.GetObjectRecursively<Image>("RepoMenuSourcesSelectionButtonRec");
        button2.sprite = whitePillBg;
        button2.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        button2.GetComponent<Button>().onClick.AddListener((System.Action)(() => OnRepoTab()));
        var button3 = transform.GetObjectRecursively<Image>("RepoMenuInstalledSelectionButtonRec");
        button3.sprite = whitePillBg;
        button3.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        var button4 = transform.GetObjectRecursively<Image>("RepoMenuSettingsSelectionButtonRec");
        button4.sprite = whitePillBg;
        button4.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        repoPanel = transform.GetObjectRecursively<Transform>("RepoViewPanelRec");
        modPanel = transform.GetObjectRecursively<Transform>("ModViewPanelRec");
        //toTranslate.Add(button1.transform.GetObjectRecursively<TextMeshProUGUI>("ModViewNameTextRec"),"thememenu.category.selector");
        //toTranslate.Add(transform.GetObjectRecursively<TextMeshProUGUI>("TitleTextRec"),"repomenu.title");
    }

    public void OnRepoTab()
    {
        modPanel.gameObject.SetActive(false);
        GameObject buttonPrefab = transform.GetObjectRecursively<GameObject>("RepoMenuTemplateButton");
        var repoContent = transform.GetObjectRecursively<Transform>("RepoMenuRepoContentRec");
        repoContent.DestroyAllChildren();
        foreach (var repo in SR2ERepoManager.repos)
        {
            if (repo.Value == null)
            {
                GameObject obj = Instantiate(buttonPrefab, repoContent);
                Button b = obj.GetComponent<Button>();
                b.transform.GetObjectRecursively<TextMeshProUGUI>("ModViewNameTextRec").text = "BROKEN: "+repo.Key;
                b.transform.GetObjectRecursively<Image>("ModViewIconImageRec").sprite = null;
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
                b.transform.GetObjectRecursively<TextMeshProUGUI>("ModViewNameTextRec").text = repo.Value.name;
                var listIcon = b.transform.GetObjectRecursively<Image>("ModViewIconImageRec");
                listIcon.sprite = null;
                if (!string.IsNullOrWhiteSpace(repo.Value.icon_url))
                    HttpEUtil.DownloadTexture2DIntoImageAsync(repo.Value.icon_url, listIcon,true,256,256);
                obj.SetActive(true);
                    
                b.onClick.AddListener((Action)(() =>
                {
                    repoPanel.gameObject.SetActive(true);
                    var name = repoPanel.GetObjectRecursively<TextMeshProUGUI>("RepoViewNameTextRec");
                    var desc = repoPanel.GetObjectRecursively<TextMeshProUGUI>("RepoViewDescriptionTextRec");
                    var hImage = repoPanel.GetObjectRecursively<Image>("RepoViewHeaderImageRec");
                    hImage.sprite = null;
                    if (!string.IsNullOrWhiteSpace(repo.Value.header_url))
                        HttpEUtil.DownloadTexture2DIntoImageAsync(repo.Value.header_url,hImage);
                    
                    if(string.IsNullOrWhiteSpace(repo.Value.name)) name.gameObject.SetActive(false);
                    else {name.gameObject.SetActive(true); name.SetText(repo.Value.name);}
                    
                    
                    if(string.IsNullOrWhiteSpace(repo.Value.description)) desc.gameObject.SetActive(false);
                    else {desc.gameObject.SetActive(true); desc.SetText("Description: "+repo.Value.description);}
                }));
            }
            catch {}
        }
    }
    public void OnBrowseTab()
    {
        modPanel.gameObject.SetActive(false);
        GameObject buttonPrefab = transform.GetObjectRecursively<GameObject>("RepoMenuTemplateButton");
        var browseContent = transform.GetObjectRecursively<Transform>("RepoMenuBrowseContentRec");
        browseContent.DestroyAllChildren();
        foreach (var repo in SR2ERepoManager.repos)
        {
            if (repo.Value == null) continue;
            foreach (var mod in repo.Value.mods)
            {
                if (mod == null) return;
                try
                {
                    GameObject obj = Instantiate(buttonPrefab, browseContent);
                    Button b = obj.GetComponent<Button>();
                    b.transform.GetObjectRecursively<TextMeshProUGUI>("ModViewNameTextRec").text = mod.name;
                    var listIcon = b.transform.GetObjectRecursively<Image>("ModViewIconImageRec");
                    listIcon.sprite = null;
                    if (!string.IsNullOrWhiteSpace(mod.icon_url))
                        HttpEUtil.DownloadTexture2DIntoImageAsync(mod.icon_url, listIcon,true,256,256);
                    obj.SetActive(true);
                    
                    b.onClick.AddListener((Action)(() =>
                    {
                        modPanel.gameObject.SetActive(true);
                        var name = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewNameTextRec");
                        var author = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewAuthorTextRec");
                        var coauthors = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewCoAuthorTextRec");
                        var desc = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewDescriptionTextRec");
                        var company = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewCompanyTextRec");
                        var trademark = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewTrademarkTextRec");
                        var team = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewTeamTextRec");
                        var copyright = modPanel.GetObjectRecursively<TextMeshProUGUI>("ModViewCopyrightTextRec");

                        var hImage = modPanel.GetObjectRecursively<Image>("ModViewHeaderImageRec");
                        hImage.sprite = null;
                        if (!string.IsNullOrWhiteSpace(mod.header_url))
                            HttpEUtil.DownloadTexture2DIntoImageAsync(mod.header_url, hImage,true);

                        var iImage = modPanel.GetObjectRecursively<Image>("ModViewIconImageRec");
                        iImage.sprite = null;
                        if (!string.IsNullOrWhiteSpace(mod.icon_url))
                            HttpEUtil.DownloadTexture2DIntoImageAsync(mod.icon_url, iImage,true,256,256);
                        
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
    public override void OnCloseUIPressed()
    {
        if (MenuEUtil.isAnyPopUpOpen) return;
        
        if(repoPanel.gameObject.activeSelf)
            repoPanel.gameObject.SetActive(false);
        else if(modPanel.gameObject.activeSelf)
            modPanel.gameObject.SetActive(false);
        else Close();
    }
    
}