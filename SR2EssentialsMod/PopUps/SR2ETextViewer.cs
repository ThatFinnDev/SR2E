using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Storage;
using UnityEngine.InputSystem;

namespace SR2E.Popups;

[InjectClass]
public class SR2ETextViewer : SR2EPopUp
{
    private string _text;
    public new static void PreAwake(GameObject obj, List<object> objects)
    {
        var comp = obj.AddComponent<SR2ETextViewer>();
        comp._text = objects[0].ToString();
        comp.ReloadFont();
        
    }
    protected override void OnOpen()
    {
        var textMesh = gameObject.GetObjectRecursively<TextMeshProUGUI>("TextViewerText");
        textMesh.SetText(_text);
    }
    
    public static void Open(string text)
    {
        if (!MenuEUtil.isAnyMenuOpen)
        {
            _Open("TextViewer",typeof(SR2ETextViewer),SR2EMenuTheme.Default,new List<object>(){text});
            return;
        }
        _Open("TextViewer",typeof(SR2ETextViewer),MenuEUtil.GetOpenMenu().GetTheme(),new List<object>(){text});
    }
    public static void Open(string text, SR2EMenuTheme theme)
    {
        _Open("TextViewer",typeof(SR2ETextViewer),theme,new List<object>(){text});
    }
    protected override void OnUpdate()
    {
        if (LKey.Escape.OnKeyDown())
            Close();
    }
}