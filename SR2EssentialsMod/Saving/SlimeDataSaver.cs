/*using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Saving;

[RegisterTypeInIl2Cpp(false)]
public class SR2ESlimeDataSaver : MonoBehaviour
{
    public void SaveData()
    {
        var model = GetComponent<IdentifiableActor>();


        var data = new SR2ESlimeData()
        {
            scaleX = transform.localScale.x,
            scaleY = transform.localScale.y,
            scaleZ = transform.localScale.z,
            zeroGrav = GetComponent<Vacuumable>()._ignoresGravity,
            velocity = new Vector3Data(GetComponent<Rigidbody>().velocity)
        };
        if (SR2ESavableDataV2.Instance.slimeSavedData.ContainsKey(model.GetActorId().Value))
            SR2ESavableDataV2.Instance.slimeSavedData[model.GetActorId().Value] = data;
        else
            SR2ESavableDataV2.Instance.slimeSavedData.Add(model.GetActorId().Value, data);

    }

    public void LoadData()
    {
        if (SR2EEntryPoint.debugLogging)
            SR2EConsole.SendMessage($"load ident debug start: {gameObject.name}");
        var id = GetComponent<IdentifiableActor>()._model.actorId;
        GetComponent<Rigidbody>().velocity = Vector3Data.ConvertBack(SR2ESavableDataV2.Instance.slimeSavedData[id.Value].velocity);
        transform.localScale = new Vector3(SR2ESavableDataV2.Instance.slimeSavedData[id.Value].scaleX, SR2ESavableDataV2.Instance.slimeSavedData[id.Value].scaleY, SR2ESavableDataV2.Instance.slimeSavedData[id.Value].scaleZ);
        GetComponent<Vacuumable>()._ignoresGravity = SR2ESavableDataV2.Instance.slimeSavedData[id.Value].zeroGrav;
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
}*/
//Broken as of SR2 0.6.0