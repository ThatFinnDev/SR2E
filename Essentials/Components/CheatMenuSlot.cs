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
        var slot = sceneContext.PlayerState.Ammo.Slots[_slotID];
        if (_amountSlider.value == 0) { _entryInput.text = ""; slot.Clear(); AudioEUtil.PlaySound(MenuSound.Error); return; }
        
        var type = LookupEUtil.GetIdentifiableTypeByName(_entryInput.text);
        if (!type) { _entryInput.text = ""; slot.Clear(); _amountSlider.value = 0; AudioEUtil.PlaySound(MenuSound.Error); return; }
        if(_radiant&&!AllowRadiant(type))
            ChangeType();
        AudioEUtil.PlaySound(MenuSound.Apply);
        string itemName = type.GetName().Replace("'","").Replace(" ","");
        _entryInput.text = itemName;
        slot.Clear();
        sceneContext.PlayerState.Ammo.MaybeAddResource(type, _slotID, (int)_amountSlider.value, true);
        slot.Radiant = _radiant;
        if (SlimeDefinition.IsSlimeDefinition(type) && _radiant)
        {
            //Refresh the appearance in the slot
            var count = slot.Count;
            slot._count++;
            slot.Count = count;
            ExecuteInTicks(() =>
            {
                var execGetter = slot.Count;
            },1);
        }
        slot.Metadata.Radiant = _radiant;
    }
    private void Select()
    {
        var slot = sceneContext.PlayerState.Ammo.Slots[_slotID];
        AudioEUtil.PlaySound(MenuSound.Click);
        var dict = new Dictionary<string, (string, Sprite)>();
        var remove = slot.Definition.SlotBlockList;
        foreach (var identType in slot.Definition.SlotTypeGroup.GetAllMembersHashSet())
        {
            if(!remove.Contains(identType))
            {
                if (SlimeDefinition.IsSlimeDefinition(identType))
                {
                    var slimeDef = identType.Cast<SlimeDefinition>();
                    dict[slimeDef.ReferenceId+"|false"] = (slimeDef.GetName(), slimeDef.icon);
                    if(slimeDef.RadiantBase)
                        dict[slimeDef.ReferenceId+"|true"] = ("Radiant"+slimeDef.GetName(), slimeDef.RadiantBase.Icon);
                }
                dict[identType.ReferenceId+"|false"] = (identType.GetName(), identType.icon);
            }
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
        if(type)
            if (SlimeDefinition.IsSlimeDefinition(type))
                foreach (var appearance in type.Cast<SlimeDefinition>().AppearancesDefault)
                    if (appearance.name.Contains("Radiant"))
                        return true;
        return false;
    }
    private bool AllowRadiant(string input)
    {
        return AllowRadiant(LookupEUtil.identifiableTypes.GetEntryByRefID(input));
    }
    private void ChangeType()
    {
        _radiant = !_radiant;
        _typeButtonText.SetText(_radiant?"Radiant":"Default");
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