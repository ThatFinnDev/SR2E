using System;
using Il2CppTMPro;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Components;

[InjectClass]
internal class CheatMenuNewbucks : MonoBehaviour
{
    private Slider amountSlider;
    private TextMeshProUGUI handleText;
    private bool didStartRan = false;
    private int dontChange = 0;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.GetObjectRecursively<Slider>("Slider");
        handleText = amountSlider.gameObject.GetObjectRecursively<TextMeshProUGUI>("Text");
        amountSlider.onValueChanged.AddListener((Action<float>)((value) =>
        {
            if (dontChange>0)
            { dontChange--; return; }
            dontChange = 0;
            int newValue = Mathf.Clamp((int)Math.Pow(value, 3.51),0,sceneContext.PlayerState._model.maxCurrency);
            handleText.SetText(newValue.ToString());
            CurrencyEUtil.SetCurrency("newbuck", newValue, newValue);
        }));
    }

    
    private void OnEnable()
    {
        if(!didStartRan) Start();
        try
        {
            double newValue = Math.Pow(CurrencyEUtil.GetCurrency("newbuck"), (1.0 / 3.51));
            if (newValue.ToString() == "NaN") newValue = 0;
            dontChange = 2;
            amountSlider.value = float.Parse(newValue.ToString());
            handleText.SetText(CurrencyEUtil.GetCurrency("newbuck").ToString());
        }
        catch { }
    }
}