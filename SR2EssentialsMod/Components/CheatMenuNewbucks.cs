using System;
using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuNewbucks : MonoBehaviour
{
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    private bool didStartRan = false;
    private int dontChange = 0;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        amountSlider.onValueChanged.AddListener((Action<float>)((value) =>
        {
            if (dontChange>0)
            { dontChange--; return; }
            dontChange = 0;
            double newValue = Math.Pow(value, 3.51);
            if (newValue < 0) newValue = 0;
            if (newValue > 9999999) newValue = 999999999;
            handleText.SetText(((int)newValue).ToString());
            SceneContext.Instance.PlayerState._model.currency = (int)newValue;
        }));
    }

    
    public void OnEnable()
    {
        if(!didStartRan) Start();
        try
        {
            double newValue = Math.Pow(SceneContext.Instance.PlayerState._model.currency, (1.0 / 3.51));
            dontChange = 2;
            amountSlider.value = float.Parse(newValue.ToString());
            handleText.SetText(SceneContext.Instance.PlayerState._model.currency.ToString());
        }
        catch { }
    }
}