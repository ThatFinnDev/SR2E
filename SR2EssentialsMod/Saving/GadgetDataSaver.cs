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
            long actorID = 0;
            foreach (var pair in gameModel.identifiables)
                if (pair.Value.GetGameObject() == gameObject)
                { actorID = pair.Key; break; }
            
            if(actorID==0) return;
            
            var data = new SR2EGadgetData()
            {
                scaleX = transform.localScale.x,
                scaleY = transform.localScale.y,
                scaleZ = transform.localScale.z,
            }; 
            if (SR2ESavableData.Instance.gadgetSavedData.ContainsKey(actorID))
            {
                SR2ESavableData.Instance.gadgetSavedData[actorID] = data;
            }
            else
            {
                SR2ESavableData.Instance.gadgetSavedData.Add(actorID, data);
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
            SR2Console.SendMessage($"load ident debug start: {gameObject.name}");
        
        
        GameModel gameModel = FindObjectOfType<GameModel>();
        long id = 0;
        foreach (var pair in gameModel.identifiables)
            if (pair.Value.GetGameObject() == gameObject)
            { id = pair.Key; break; }
            
        if(id==0) return;
        
        transform.localScale = new Vector3(SR2ESavableData.Instance.gadgetSavedData[id].scaleX, SR2ESavableData.Instance.gadgetSavedData[id].scaleY, SR2ESavableData.Instance.gadgetSavedData[id].scaleZ);
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
