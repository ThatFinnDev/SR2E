using System;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppTMPro;
using Starlight.Enums.Sounds;
using Starlight.Popups;
using Starlight.Storage;
using Unity.Mathematics;
using UnityEngine.UI;

namespace Starlight.Components;

[InjectIntoIL]
internal class CheatMenuSlot : MonoBehaviour
{
    private Button _applyButton;
    private Button _selectButton;
    private Button _typeButton;
    private Slider _amountSlider;
    private TextMeshProUGUI _handleText;
    private TextMeshProUGUI _typeButtonText;
    private TMP_InputField _entryInput;
    private bool _radiant = false;
    private int _slotID;

    private void Apply()
    {
        if (_amountSlider.value == 0) { _entryInput.text = ""; InventoryEUtil.ClearSlot(_slotID); AudioEUtil.PlaySound(MenuSound.Error); return; }
        
        var type = LookupEUtil.GetIdentifiableTypeByName(_entryInput.text);
        if (!type) { _entryInput.text = ""; InventoryEUtil.ClearSlot(_slotID); _amountSlider.value = 0; AudioEUtil.PlaySound(MenuSound.Error); return; }
        if(_radiant&&!AllowRadiant(type))
            ChangeType();
        AudioEUtil.PlaySound(MenuSound.Apply);
        string itemName = type.GetName().Replace("'","").Replace(" ","");
        _entryInput.text = itemName;
        InventoryEUtil.SetStarlightSlotInfo(_slotID,new StarlightSlotItemInfo()
        {
            Count = (int)_amountSlider.value,
            IdentifiableType = type,
            IsRadiant = _radiant
        });
    }

    void Select_RadiantStuff(Dictionary<string, (string, Sprite)> dict, SlimeDefinition slimeDef)
    {
        dict[slimeDef.ReferenceId+"|true"] = ("Radiant"+slimeDef.GetName(), slimeDef.RadiantBase.Icon);
    }
    private void Select()
    {
        var slot = sceneContext.PlayerState.Ammo.Slots[_slotID];
        AudioEUtil.PlaySound(MenuSound.Click);
        var dict = new Dictionary<string, (string, Sprite)>();
        foreach (var identType in InventoryEUtil.GetSlotAllowedIdentifiableTypes(_slotID))
        {
            if (SlimeDefinition.IsSlimeDefinition(identType))
            {
                var slimeDef = identType.Cast<SlimeDefinition>();
                dict[slimeDef.ReferenceId+"|false"] = (slimeDef.GetName(), slimeDef.icon);
                if (AllowRadiant(slimeDef))
                    Select_RadiantStuff(dict, slimeDef);
            }
            dict[identType.ReferenceId+"|false"] = (identType.GetName(), identType.icon);
            
        }
        StarlightGridMenuListPopUp.Open(dict, (value) =>
        {
            if (_amountSlider.value == 0)
                _amountSlider.value = 1;
            _entryInput.SetText(slot.Definition.SlotTypeGroup.GetAllMembersHashSet().ToNetArray().GetEntryByRefID(value.Split("|")[0]).GetName());
            var allowRadiant = AllowRadiant(value.Split("|")[0]);
            var useRadiant = value.Split("|")[1] == "true";
            if(_radiant&&(!allowRadiant||!useRadiant)) ChangeType();
            if(!_radiant&&allowRadiant&&useRadiant) ChangeType();
        });
    }
    private bool AllowRadiant(IdentifiableType type)
    {
        if (!SupportRadiant.HasFlag()) return false;
        if(type)
            if (SlimeDefinition.IsSlimeDefinition(type))
                foreach (var appearance in type.Cast<SlimeDefinition>().AppearancesDefault)
                    if (appearance.name.Contains("Radiant"))
                        return true;
        return false;
    }
    private bool AllowRadiant(string input)
    {
        if (!SupportRadiant.HasFlag()) return false;
        return AllowRadiant(LookupEUtil.identifiableTypes.GetEntryByRefID(input));
    }
    private void ChangeType()
    {
        if (SupportRadiant.HasFlag())
        {
            _radiant = !_radiant;
            _typeButtonText.SetText(_radiant?"Radiant":"Default");
        }
        else
        {
            _radiant = false;
            _typeButtonText.SetText("Default");
        }
    }
    internal void OnOpen(int id)
    {
        _slotID = id;
        gameObject.GetObjectRecursively<TextMeshProUGUI>("Text").SetText(" Slot "+(id+1)+":");
        _applyButton = gameObject.GetObjectRecursively<Button>("Apply");
        _selectButton = gameObject.GetObjectRecursively<Button>("Select");
        _typeButton = gameObject.GetObjectRecursively<Button>("Type");
        _typeButtonText = _typeButton.gameObject.GetObjectRecursively<TextMeshProUGUI>("Text");
        _amountSlider = gameObject.GetObjectRecursively<Slider>("Slider");
        _handleText = _amountSlider.gameObject.GetObjectRecursively<TextMeshProUGUI>("Text");
        _entryInput = gameObject.GetObjectRecursively<TMP_InputField>("EntryInput");
        _applyButton.onClick.AddListener((SystemAction)(Apply));
        _typeButton.onClick.AddListener((SystemAction)(ChangeType));
        _selectButton.onClick.AddListener((SystemAction)(Select));
        _amountSlider.onValueChanged.AddListener((Action<float>)((value) => { _handleText.SetText(((int)value).ToString()); }));
        
        var slot = sceneContext.PlayerState.Ammo.Slots[_slotID];
        if (slot == null) return;
        if(slot.Radiant) ChangeType();
        _amountSlider.maxValue = slot.MaxCount;
        _amountSlider.value = slot.Count;
        string identName = "";
        if (slot.Id) identName = slot.Id.GetName().Replace("'","").Replace(" ","");
        
        _entryInput.text = identName;
    }
}