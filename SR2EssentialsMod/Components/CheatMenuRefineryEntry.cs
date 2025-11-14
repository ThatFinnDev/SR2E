using System;
using Il2CppMonomiPark.SlimeRancher.Economy;
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
        amountSlider = gameObject.GetObjectRecursively<Slider>("Slider");
        amountSlider.maxValue = GadgetDirector.REFINERY_MAX;
        handleText = amountSlider.gameObject.GetObjectRecursively<TextMeshProUGUI>("Text");
        itemName = gameObject.GetObjectRecursively<TextMeshProUGUI>("Name");
        icon = gameObject.GetObjectRecursively<Image>("Icon");
        icon.sprite = item.icon;
        itemName.text = item.GetName(false);
        amountSlider.onValueChanged.AddListener((Action<float>)((valueFloat) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            int newValue = (int)valueFloat;
            
            //Adding one updates the new value everywhere. Not doing can cause issues
            sceneContext.GadgetDirector._model.SetCount(item,newValue-1);
            sceneContext.GadgetDirector.AddItem(item,1);
            
            handleText.SetText(newValue.ToString());
        }));}

    public void OnOpen()
    {
        if(!didStartRan) Start();

        dontChange = true;
        amountSlider.value = sceneContext.GadgetDirector.GetItemCount(item);
        handleText.SetText(sceneContext.GadgetDirector.GetItemCount(item).ToString());
    }
}