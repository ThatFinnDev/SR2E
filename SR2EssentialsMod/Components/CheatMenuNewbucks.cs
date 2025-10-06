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
            int newValue = Mathf.Clamp((int)Math.Pow(value, 3.51),0,SceneContext.Instance.PlayerState._model.maxCurrency);
            handleText.SetText(newValue.ToString());
            SetCurrency("newbuck", newValue, newValue);
        }));
    }

    
    public void OnEnable()
    {
        if(!didStartRan) Start();
        try
        {
            double newValue = Math.Pow(GetCurrency("newbuck"), (1.0 / 3.51));
            if (newValue.ToString() == "NaN") newValue = 0;
            dontChange = 2;
            amountSlider.value = float.Parse(newValue.ToString());
            handleText.SetText(GetCurrency("newbuck").ToString());
        }
        catch { }
    }
}