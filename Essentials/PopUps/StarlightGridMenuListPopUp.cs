
using System;
using Il2CppTMPro;
using Starlight.Enums;
using Starlight.Enums.Sounds;
using Starlight.Storage;
using UnityEngine.UI;

namespace Starlight.Popups;

[InjectIntoIL]
public class StarlightGridMenuListPopUp : StarlightPopUp
{
    private Dictionary<string,(string, Sprite)> _entries;
    private Action<string> _onSelect;
    public void OnPress(string key)
    {
        _onSelect.Invoke(key);
        Close();
    }
    public new static void PreAwake(GameObject obj, List<object> objects)
    {
        var comp = obj.AddComponent<StarlightGridMenuListPopUp>();
        comp._entries = (Dictionary<string,(string, Sprite)>) objects[0];
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
            instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (value.Item1);
            instance.transform.GetChild(1).GetComponent<Image>().sprite = value.Item2;
            instance.onClick.AddListener((Action)(() =>
            {
                AudioEUtil.PlaySound(MenuSound.Click);
                OnPress(entry.Key);
            }));
        }

    }
    public static void Open(Dictionary<string, (string, Sprite)> entries,Action<string> onSelect)
    {
        if (!MenuEUtil.isAnyMenuOpen)
        {
            _Open("GridMenuList",typeof(StarlightGridMenuListPopUp),StarlightMenuTheme.Starlight,new List<object>(){entries,onSelect});
            return;
        }
        _Open("GridMenuList",typeof(StarlightGridMenuListPopUp),MenuEUtil.GetOpenMenu().GetTheme(),new List<object>(){entries,onSelect});
    }
    public static void Open(Dictionary<string, (string, Sprite)> entries,Action<string> onSelect, StarlightMenuTheme theme)
    {
        _Open("GridMenuList",typeof(StarlightGridMenuListPopUp),theme,new List<object>(){entries,onSelect});
    }
    protected override void OnUpdate()
    {
        if (LKey.Escape.OnKeyDown())
            Close();
    }
}