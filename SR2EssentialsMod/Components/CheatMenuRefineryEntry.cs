using System;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuRefineryEntry : MonoBehaviour
{
    public IdentifiableType item;
    public Image icon;
    private bool dontChange = false;
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    public TextMeshProUGUI itemName;
    private bool didStartRan = false;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        amountSlider.maxValue = GadgetDirector.REFINERY_MAX;
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        itemName = gameObject.getObjRec<TextMeshProUGUI>("Name");
        icon = gameObject.getObjRec<Image>("Icon");
        icon.sprite = item.icon;
        itemName.text = item.getName().Replace("'", "");
        amountSlider.onValueChanged.AddListener((Action<float>)((valueFloat) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            int newValue = int.Parse(valueFloat.ToString().Split(".")[0]);
            int oldValue = SceneContext.Instance.GadgetDirector.GetItemCount(item);
            int difference = newValue - oldValue;
            if (difference == 0) return;
            if (difference > 0)
                SceneContext.Instance.GadgetDirector.AddItem(item,difference);
            if (difference < 0)
            {
                IdentCostEntry costEntry = new IdentCostEntry();
                costEntry.amount = -difference;
                costEntry.identType = item;
                Il2CppSystem.Collections.Generic.List<IdentCostEntry> entries =
                    new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
                entries.Add(costEntry);
                SceneContext.Instance.GadgetDirector.TryToSpendItems(entries);
            }
            handleText.SetText(newValue.ToString());
        }));}

    public void OnOpen()
    {
        if(!didStartRan) Start();

        dontChange = true;
        amountSlider.value = SceneContext.Instance.GadgetDirector.GetItemCount(item);
        handleText.SetText(SceneContext.Instance.GadgetDirector.GetItemCount(item).ToString());
    }
}