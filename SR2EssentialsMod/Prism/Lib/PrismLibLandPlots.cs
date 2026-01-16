using System.Collections;
using Il2CppMonomiPark.SlimeRancher.Regions;
using SR2E.Prism.Data.LandPlots;
using UnityEngine.SceneManagement;

namespace SR2E.Prism.Lib;

public static class PrismLibLandPlots
{
    internal static Dictionary<string, PrismLandPlotLocation> customPlots = new ();
    internal static Dictionary<string, GameObject> rootObjects = new ();
    internal static List<LandPlotLocation> landPlotLocations = new();
    internal static GameObject GetNewLandPlotRoot(string sceneName)
    {
        if(rootObjects.ContainsKey(sceneName))
            if (rootObjects[sceneName] != null)
                return rootObjects[sceneName];
        var gameObj = new GameObject("PrismLandPlotRoots-" + sceneName);
        SceneManager.MoveGameObjectToScene(gameObj, SceneManager.GetSceneByName(sceneName));
        var foundACell = false;
        foreach (var dir in GetAllInScene<CellDirector>())
        {
            if (dir.gameObject.scene.name == sceneName)
            {
                gameObj.transform.SetParent(dir.transform);
                foundACell = true;
                break;
            }
        }
        if(!foundACell)
        {
            var anyDir = GetAnyInScene<CellDirector>();
            if(anyDir==null) MelonLogger.Msg("Oh oh... A landplot is outside a CellDirector. Things are about to get sideways");
            else gameObj.transform.SetParent(anyDir.transform);
        }
        rootObjects[sceneName] = gameObj;
        return gameObj;
    }
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        ExecuteInTicks((() =>
        {
            if (sceneName == "MainMenuUI")
            {
                landPlotLocations = new();
                customPlots = new();
                rootObjects = new();
            }
            foreach (var plot in customPlots)
            {
                if (plot.Value.sceneName == sceneName)
                {
                    try
                    {
                        SpawnLandPlot(plot.Key, plot.Value);
                    }
                    catch (Exception e) { MelonLogger.Error(e); }
                }
            }
        }),2);
    }
    /// <summary>
    /// Note, custom landplots don't support drones yet!
    /// </summary>
    /// <param name="id"></param>
    /// <param name="loc"></param>
    public static void AddLandPlotLocation(string id,PrismLandPlotLocation loc)
    {
        if (!inGame) return;
        if (loc == null || string.IsNullOrWhiteSpace(id)) return;
        var scene = SceneManager.GetSceneByName(loc.sceneName);
        if (scene == null) return;
        if (customPlots.ContainsKey(id)) return;
         
        customPlots.Add(id,loc);
        if (scene.isLoaded)
            SpawnLandPlot(id, loc);

    }

    public static bool HasLandPlotLocation(string id) => customPlots.ContainsKey(id);
    public static void RemoveLandPlotLocation(string id)
    {
        if (!inGame) return;
        if (string.IsNullOrWhiteSpace(id)) return;
        if (!customPlots.ContainsKey(id)) return;
        customPlots.Remove(id);
        try { GameObject.Destroy(sceneContext.GameModel.landPlots[id].gameObj); } catch { }
        try { GameObject.Destroy(sceneContext.GameModel.landPlots["plot"+id].gameObj); } catch { }
        try { sceneContext.GameModel.UnregisterLandPlot(id); }catch { }
        try { sceneContext.GameModel.UnregisterLandPlot("plot"+id); }catch { }
        if (sceneContext.GameModel.landPlots.ContainsKey(id))
            sceneContext.GameModel.landPlots.Remove(id);
        if (sceneContext.GameModel.landPlots.ContainsKey("plot"+id))
            sceneContext.GameModel.landPlots.Remove(id);
    }

    static void SpawnLandPlot(string plotKey, PrismLandPlotLocation loc)
    { 
        GameObject landplotRoot = GetNewLandPlotRoot(loc.sceneName);
        var obj = new GameObject(plotKey);
        var lpl = obj.AddComponent<LandPlotLocation>();
        landPlotLocations.Add(lpl);
        lpl._id = "plot" + plotKey;
        obj.transform.SetParent(landplotRoot.transform);
        obj.transform.position = loc.position;
        obj.transform.rotation = loc.rotation;
        obj.transform.localScale = loc.scale;
        var id = loc.defaultPlot;
        if (id == LandPlot.Id.NONE) id = LandPlot.Id.EMPTY;        
        ExecuteInTicks(() =>
        {
            var prefab = gameContext.LookupDirector.GetPlotPrefab(id);
            var plotObj = GameObject.Instantiate(prefab, obj.transform);
            lpl.enabled = true;
            ExecuteInTicks(() =>
            {
                var landPlot = plotObj.GetComponent<LandPlot>();
                try { sceneContext.GameModel.RegisterLandPlot(lpl._id,obj); }catch { }
                //Yes, it's a different key on purpose
                landPlot.InitModel(sceneContext.GameModel.InitializeLandPlotModel(plotKey));
            },2);
        },2);
    }
}