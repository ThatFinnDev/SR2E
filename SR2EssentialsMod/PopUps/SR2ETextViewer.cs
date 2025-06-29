using System;
using Il2CppTMPro;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E.Popups;

[RegisterTypeInIl2Cpp(false)]
public class SR2ETextViewer : SR2EPopUp
{
    private string _text;
    public new static void PreAwake(GameObject obj, List<object> objects)
    {
        var comp = obj.AddComponent<SR2ETextViewer>();
        comp._text = objects[0].ToString();
    }
    protected override void OnOpen()
    {
        var textMesh = gameObject.getObjRec<TextMeshProUGUI>("TextViewerText");
        textMesh.SetText(_text);
    }
    
    public static void Open(string text)
    {
        if (!isAnyMenuOpen) return;
        _Open("TextViewer",typeof(SR2ETextViewer),getOpenMenu.GetTheme(),new List<object>(){text});
    }

    protected override void OnUpdate()
    {
        if (Key.Escape.OnKeyPressed())
            Close();
    }
}