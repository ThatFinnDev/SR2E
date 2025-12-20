
using System;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Enums.Sounds;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E.Popups;

[InjectClass]
public class SR2EGridMenuList : SR2EPopUp
{
    private TripleDictionary<string,string, Sprite> _entries;
    private Action<string> _onSelect;
    public void OnPress(string key)
    {
        _onSelect.Invoke(key);
        Close();
    }
    public new static void PreAwake(GameObject obj, List<object> objects)
    {
        var comp = obj.AddComponent<SR2EGridMenuList>();
        comp._entries = (TripleDictionary<string,string, Sprite>) objects[0];
        comp._onSelect = (Action<string>) objects[1];
        comp.ReloadFont();
    }
    protected override void OnOpen()
    {
        var content = gameObject.GetObjectRecursively<Transform>("MenuListContentRec");
        var prefab = gameObject.GetObjectRecursively<Button>("MenuListTemplateEntry");
        foreach (var entry in _entries)
        {
            var value = entry.Value;
            var instance = GameObject.Instantiate(prefab, content.transform);
            instance.gameObject.SetActive(true);
            instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(value.Item1);
            instance.transform.GetChild(1).GetComponent<Image>().sprite = value.Item2;
            instance.onClick.AddListener((Action)(() =>
            {
                AudioEUtil.PlaySound(MenuSound.Click);
                OnPress(entry.Key);
            }));
        }

    }
    public static void Open(TripleDictionary<string,string,Sprite> entries,Action<string> onSelect)
    {
        if (!MenuEUtil.isAnyMenuOpen)
        {
            _Open("GridMenuList",typeof(SR2EGridMenuList),SR2EMenuTheme.Default,new List<object>(){entries,onSelect});
            return;
        }
        _Open("GridMenuList",typeof(SR2EGridMenuList),MenuEUtil.GetOpenMenu().GetTheme(),new List<object>(){entries,onSelect});
    }
    public static void Open(TripleDictionary<string,string,Sprite> entries,Action<string> onSelect, SR2EMenuTheme theme)
    {
        _Open("GridMenuList",typeof(SR2EGridMenuList),theme,new List<object>(){entries,onSelect});
    }
    protected override void OnUpdate()
    {
        if (LKey.Escape.OnKeyDown())
            Close();
    }
}