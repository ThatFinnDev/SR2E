namespace SR2E.Saving;

[RegisterTypeInIl2Cpp(false)]
public class SR2EGordoDataSaver : MonoBehaviour
{
    public void SaveData()
    {
        var model = GetComponent<GordoEat>();

        var data = new SR2EGordoData()
        {
            baseSize = model._initScale
        };

        if (SR2ESavableDataV2.Instance.gordoSavedData.ContainsKey(model.Id))
            SR2ESavableDataV2.Instance.gordoSavedData[model.Id] = data;
        else
            SR2ESavableDataV2.Instance.gordoSavedData.Add(model.Id, data);
        
    }

    public void LoadData()
    {
        if (SR2EEntryPoint.debugLogging)
            SR2EConsole.SendMessage($"load ident debug start: {gameObject.name}");
        var id = GetComponent<GordoEat>().Id;
        GetComponent<GordoEat>()._initScale = SR2ESavableDataV2.Instance.gordoSavedData[id].baseSize;
        if (SR2EEntryPoint.debugLogging)
            SR2EConsole.SendMessage("loaded ident");
    }
    public void Start()
    {
        try
        {
            LoadData();
        }
        catch { }
    }
}
