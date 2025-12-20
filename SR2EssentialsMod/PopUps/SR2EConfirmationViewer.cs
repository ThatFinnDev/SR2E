using System;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Popups;


[InjectClass]
public class SR2EConfirmationViewer : SR2EPopUp
{
    private string _text;
    private int variant = 0;
    private Action okAction = null;
    private Action yesAction = null;
    private Action noAction = null;
    private Action escapeAction = null;
    public new static void PreAwake(GameObject obj, List<object> objects)
    {
        var comp = obj.AddComponent<SR2EConfirmationViewer>();
        comp._text = objects[0].ToString();
        comp.variant= int.Parse(objects[1].ToString());
        
        comp.ReloadFont();
        
    }
    protected override void OnOpen()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        var okButton = gameObject.GetObjectRecursively<Button>("OKButtonRec");
        var yesButton = gameObject.GetObjectRecursively<Button>("YesButtonRec");
        var noButton = gameObject.GetObjectRecursively<Button>("NoButtonRec");
        if (variant == 0)
        {
            okButton.gameObject.SetActive(true);
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
            okButton.onClick.AddListener((Action)(() =>
            {
                if(okAction!=null) okAction.Invoke();
                Close();
            }));
        }
        else if (variant == 1)
        {
            okButton.gameObject.SetActive(false);
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            yesButton.onClick.AddListener((Action)(() =>
            {
                if(yesAction!=null) yesAction.Invoke();
                Close();
            }));
            noButton.onClick.AddListener((Action)(() =>
            {
                if(noAction!=null) noAction.Invoke();
                Close();
            }));
        }
        var textMesh = gameObject.GetObjectRecursively<TextMeshProUGUI>("TextViewerText");
        textMesh.SetText(_text);
    }
    
    
    
    public static void Open(string text, Action yesAction, Action noAction, Action escapeAction)
    {
        if (!MenuEUtil.isAnyMenuOpen)
        {
            _Open("ConfirmationViewer",typeof(SR2EConfirmationViewer),SR2EMenuTheme.Default,new List<object>(){text,1,yesAction,noAction,escapeAction});
            return;
        }
        _Open("ConfirmationViewer",typeof(SR2EConfirmationViewer),MenuEUtil.GetOpenMenu().GetTheme(),new List<object>(){text,1,yesAction,noAction,escapeAction});
    }
    public static void Open(string text, Action yesAction, Action noAction, Action escapeAction, SR2EMenuTheme theme)
    {
        _Open("ConfirmationViewer",typeof(SR2EConfirmationViewer),theme,new List<object>(){text,1,yesAction,noAction,escapeAction});
    }
    
    
    public static void Open(string text, Action okAction, Action escapeAction)
    {
        if (!MenuEUtil.isAnyMenuOpen)
        {
            _Open("ConfirmationViewer",typeof(SR2EConfirmationViewer),SR2EMenuTheme.Default,new List<object>(){text,0,okAction,escapeAction});
            return;
        }
        _Open("ConfirmationViewer",typeof(SR2EConfirmationViewer),MenuEUtil.GetOpenMenu().GetTheme(),new List<object>(){text,0,okAction,escapeAction});
    }
    public static void Open(string text, Action okAction, Action escapeAction, SR2EMenuTheme theme)
    {
        _Open("ConfirmationViewer",typeof(SR2EConfirmationViewer),theme,new List<object>(){text,0,okAction,escapeAction});
    }
    
    
    
    protected override void OnUpdate()
    {
        if (LKey.Escape.OnKeyDown())
        {
            if(escapeAction!=null) escapeAction.Invoke();
            Close();
        }
    }
}