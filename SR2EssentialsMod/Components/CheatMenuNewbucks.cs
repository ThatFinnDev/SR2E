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
    private bool dontChange = false;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        amountSlider.onValueChanged.AddListener((Action<float>)((value) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            double newValue = Math.Pow(value, 4.5);
            if (newValue < 0) newValue = 0;
            if (newValue > 9999999999) newValue = 9999999999;
            handleText.SetText(newValue.ToString().Split(".")[0]);
            SceneContext.Instance.PlayerState._model.currency = int.Parse(newValue.ToString().Split(".")[0]);
        }));
    }

    
    public void OnEnable()
    {
        if(!didStartRan) Start();

        try
        {
            double newValue = Math.Pow(SceneContext.Instance.PlayerState._model.currency, (1.0 / 4.5));
            dontChange = true;
            amountSlider.value = int.Parse(newValue.ToString().Split(".")[0]);
            handleText.SetText(SceneContext.Instance.PlayerState._model.currency.ToString());
        }
        catch { }
    }
}