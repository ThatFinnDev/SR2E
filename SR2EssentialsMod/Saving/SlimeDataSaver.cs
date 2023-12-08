using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;

namespace SR2E.Saving;

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
            zeroGrav = GetComponent<Vacuumable>().ignoresGravity,
            velocity = new Vector3Data(GetComponent<SRCharacterController>().Velocity)
        }; 
        if (SR2ESavableData.Instance.slimeSavedData.ContainsKey(model.GetActorId()))
        {
            SR2ESavableData.Instance.slimeSavedData[model.GetActorId()] = data;
        }
        else
        {
            SR2ESavableData.Instance.slimeSavedData.Add(model.GetActorId(), data);
        }
    }

    public void LoadData()
    {
        if (SR2EEntryPoint.debugLogging)
            SR2Console.SendMessage($"load ident debug start: {gameObject.name}");
        var id = GetComponent<IdentifiableActor>().model.actorId;
        GetComponent<Rigidbody>().velocity = Vector3Data.ConvertBack(SR2ESavableData.Instance.slimeSavedData[id].velocity);
        transform.localScale = new Vector3(SR2ESavableData.Instance.slimeSavedData[id].scaleX, SR2ESavableData.Instance.slimeSavedData[id].scaleY, SR2ESavableData.Instance.slimeSavedData[id].scaleZ);
        GetComponent<Vacuumable>().ignoresGravity = SR2ESavableData.Instance.slimeSavedData[id].zeroGrav;
        if (SR2EEntryPoint.debugLogging)
            SR2Console.SendMessage("loaded ident");
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
