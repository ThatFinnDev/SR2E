using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Utils;

public static class SpawnEUtil
{        
    public static GadgetModel SpawnGadget(this GadgetDefinition def, Vector3 pos) => SpawnGadget(def, pos, Quaternion.identity);
    public static GadgetModel SpawnGadget(this GadgetDefinition def, Vector3 pos, Vector3 rot)=> SpawnGadget(def, pos, Quaternion.Euler(rot));
    public static GadgetModel SpawnGadget(this GadgetDefinition def, Vector3 pos, Quaternion rot)
    {
        if (def == null) return null;
        var modelGadget = sceneContext.GameModel.InstantiateGadgetModel(def, systemContext.SceneLoader.CurrentSceneGroup, pos);
        GadgetDirector.InstantiateGadgetFromModel(modelGadget);
        modelGadget.eulerRotation = rot.ToEuler();
        return modelGadget;
    }
    public static GameObject SpawnActor(this IdentifiableType ident, Vector3 pos) => SpawnActor(ident, pos, Quaternion.identity);
    public static GameObject SpawnActor(this IdentifiableType ident, Vector3 pos, Vector3 rot)=> SpawnActor(ident, pos, Quaternion.Euler(rot));
    public static GameObject SpawnActor(this IdentifiableType ident, Vector3 pos, Quaternion rot)
    {
        if (ident == null) return null;
        if (ident.TryCast<GadgetDefinition>()!=null) return SpawnGadget(ident.TryCast<GadgetDefinition>(), pos, rot).GetGameObject();
        return InstantiationHelpers.InstantiateActor(ident.prefab, sceneContext.RegionRegistry.CurrentSceneGroup, pos, rot);
    }
    public static GameObject SpawnDynamic(this GameObject obj, Vector3 pos, Quaternion rot)
    {
        return InstantiationHelpers.InstantiateDynamic(obj, pos, rot);
    }
        
    public static GameObject SpawnFX(this GameObject fx, Vector3 pos) => SpawnFX(fx, pos, Quaternion.identity);
        
    public static GameObject SpawnFX(this GameObject fx, Vector3 pos, Quaternion rot)
    {
        return FXHelpers.SpawnFX(fx, pos, rot);
    }
}