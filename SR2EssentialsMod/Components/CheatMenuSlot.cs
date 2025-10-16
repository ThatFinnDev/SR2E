using System;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppTMPro;
using SR2E.Popups;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuSlot : MonoBehaviour
{
    public int slotID;
    public Button applyButton;
    public Button selectButton;
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    public TMP_InputField entryInput;
    private bool didStartRan = false;
    private void Start()
    {
        if (didStartRan) return;
        didStartRan = true;
        slotID = int.Parse(gameObject.GetObjectRecursively<TextMeshProUGUI>("Text").text.Replace(" ", "").Replace(":", "").Replace("Slot", ""))-1;
        applyButton = gameObject.GetObjectRecursively<Button>("Apply");
        selectButton = gameObject.GetObjectRecursively<Button>("Select");
        amountSlider = gameObject.GetObjectRecursively<Slider>("Slider");
        handleText = amountSlider.gameObject.GetObjectRecursively<TextMeshProUGUI>("Text");
        entryInput = gameObject.GetObjectRecursively<TMP_InputField>("EntryInput");
        applyButton.onClick.AddListener((Action)(() =>{Apply();}));
        selectButton.onClick.AddListener((Action)(() =>{Select();}));
        amountSlider.onValueChanged.AddListener((Action<float>)((value) => { handleText.SetText(((int)value).ToString()); }));
    }

    public void Apply()
    {
        if (amountSlider.value == 0) { entryInput.text = ""; slot.Clear(); return; }
        
        IdentifiableType type = LookupEUtil.GetIdentifiableTypeByName(entryInput.text);
        if (type == null) { entryInput.text = ""; slot.Clear(); amountSlider.value = 0; return; }
        
        string itemName = type.GetName().Replace("'","").Replace(" ","");
        entryInput.text = itemName;
        slot.Clear();
        SceneContext.Instance.PlayerState.Ammo.MaybeAddToSpecificSlot(type, null, slotID, 
            (int)amountSlider.value);
    }
    public void Select()
    {
        var dict = new TripleDictionary<string, string, Sprite>();
        foreach (IdentifiableType identType in LookupEUtil.vaccableTypes)
        {
            if (identType.isGadget()) continue;
            if (identType.ReferenceId.ToLower() == "none" || identType.ReferenceId.ToLower() == "player") continue;
            try
            {if (identType.LocalizedName != null)
                {
                    string localizedString = identType.LocalizedName.GetLocalizedString();
                    if(localizedString.StartsWith("!")) continue;
                    dict.Add(identType.GetName().Replace("'","").Replace(" ",""), (localizedString, identType.icon));
                }
            }catch { }
        }
        SR2EGridMenuList.Open(dict, (Action<string>)((value) => { entryInput.SetText(value); }));
    }
    private AmmoSlot slot {
        get
        {
            try { return SceneContext.Instance.PlayerState.Ammo.Slots[slotID]; }
            catch { return null; }
        }
    }
    public void OnOpen()
    {
        if(!didStartRan) Start();
        
        if (slot == null) return;

        amountSlider.maxValue = slot.MaxCount;
        amountSlider.value = slot.Count;
        string identName = "";
        if (slot.Id != null) identName = slot.Id.GetName().Replace("'","").Replace(" ","");
        
        entryInput.text = identName;
    }
}