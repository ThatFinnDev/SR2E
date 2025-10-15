using System;
using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuGadgetEntry : MonoBehaviour
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
        
        itemName.text = item.GetName().Replace("'", "");
        amountSlider.onValueChanged.AddListener((Action<float>)((valueFloat) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            int newValue = (int)valueFloat;
            if (newValue > 0 && !SceneContext.Instance.GadgetDirector.HasBlueprint(item.Cast<GadgetDefinition>()))
                SceneContext.Instance.GadgetDirector.AddBlueprint(item.Cast<GadgetDefinition>());
            
            //Adding one updates the new value everywhere. Not doing can cause issues
            SceneContext.Instance.GadgetDirector._model.SetCount(item,newValue-1);
            SceneContext.Instance.GadgetDirector.AddItem(item,1);
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