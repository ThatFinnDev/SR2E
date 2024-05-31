using Il2CppMonomiPark.SlimeRancher.DataModel;

namespace SR2E.Saving;

[RegisterTypeInIl2Cpp(false)]
public class SR2EGadgetDataSaver : MonoBehaviour
{
    public void SaveData()
    {
        try
        {
            GameModel gameModel = FindObjectOfType<GameModel>();
            ActorId actorID = new ActorId();
            bool gotActor = false;
            foreach (var pair in gameModel.identifiables)
                if (pair.Value.GetGameObject() == gameObject)
                {
                    gotActor = true; 
                    actorID = pair.Key; break; 
                }
            
            if(!gotActor) return;
            
            var data = new SR2EGadgetData()
            {
                scaleX = transform.localScale.x,
                scaleY = transform.localScale.y,
                scaleZ = transform.localScale.z,
            }; 
            if (SR2ESavableDataV2.Instance.gadgetSavedData.ContainsKey(actorID.Value))
            {
                SR2ESavableDataV2.Instance.gadgetSavedData[actorID.Value] = data;
            }
            else
            {
                SR2ESavableDataV2.Instance.gadgetSavedData.Add(actorID.Value, data);
            }
        }
        catch (Exception e)
        {
            MelonLogger.Error(e);
            throw;
        }
    }

    public void LoadData()
    {
        if (SR2EEntryPoint.debugLogging)
            SR2EConsole.SendMessage($"load ident debug start: {gameObject.name}");
        
        
        GameModel gameModel = FindObjectOfType<GameModel>();
        ActorId id = new ActorId();
        bool gotActor = false;
        foreach (var pair in gameModel.identifiables)
            if (pair.Value.GetGameObject() == gameObject)
            {
                gotActor = true; 
                id = pair.Key; break; 
            }
            
        if(!gotActor) return;
        
        transform.localScale = new Vector3(SR2ESavableDataV2.Instance.gadgetSavedData[id.Value].scaleX, SR2ESavableDataV2.Instance.gadgetSavedData[id.Value].scaleY, SR2ESavableDataV2.Instance.gadgetSavedData[id.Value].scaleZ);
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
