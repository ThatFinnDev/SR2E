using System;
using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuSlot : MonoBehaviour
{
    public int slotID;
    public Button applyButton;
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    public TMP_InputField entryInput;
    private bool didStartRan = false;
    private void Start()
    {
        didStartRan = true;
        slotID = int.Parse(gameObject.getObjRec<TextMeshProUGUI>("Text").text.Replace(" ", "").Replace(":", "").Replace("Slot", ""))-1;
        applyButton = gameObject.getObjRec<Button>("Apply");
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        entryInput = gameObject.getObjRec<TMP_InputField>("EntryInput");
        applyButton.onClick.AddListener((Action)(() =>{Apply();}));
        amountSlider.onValueChanged.AddListener((Action<float>)((value) => { handleText.SetText(((int)value).ToString()); }));
    }

    public void Apply()
    {
        if (amountSlider.value == 0) { entryInput.text = ""; slot.Clear(); return; }
        
        IdentifiableType type = getIdentByName(entryInput.text);
        if (type == null) { entryInput.text = ""; slot.Clear(); amountSlider.value = 0; return; }
        
        string itemName = type.getName().Replace("'","").Replace(" ","");
        entryInput.text = itemName;
        slot.Clear();
        SceneContext.Instance.PlayerState.Ammo.MaybeAddToSpecificSlot(type, null, slotID, 
            (int)amountSlider.value);
    }
    private Ammo.Slot slot {
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
        if (slot.Id != null) identName = slot.Id.getName().Replace("'","").Replace(" ","");
        
        entryInput.text = identName;
    }
}