using SR2E.Saving;

namespace SR2E.Prism.Data.LandPlots;

public class PrismLandPlotLocation : SubSave
{
    [StoreInSave] public Vector3 position;
    [StoreInSave] public Quaternion rotation;
    [StoreInSave] public Vector3 scale;
    [StoreInSave] public string sceneName;
    [StoreInSave] public LandPlot.Id defaultPlot;

    public PrismLandPlotLocation(Vector3 position, string sceneName, LandPlot.Id defaultPlot)
    {
        this.position = position;
        this.scale = new Vector3(1,1,1);
        this.sceneName = sceneName;
        this.defaultPlot = defaultPlot;
    }
    public PrismLandPlotLocation(Vector3 position, Quaternion rotation, string sceneName, LandPlot.Id defaultPlot)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = new Vector3(1,1,1);
        this.sceneName = sceneName;
        this.defaultPlot = defaultPlot;
    }
    public PrismLandPlotLocation(Vector3 position, Quaternion rotation, Vector3 scale, string sceneName, LandPlot.Id defaultPlot)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.sceneName = sceneName;
        this.defaultPlot = defaultPlot;
    }
    public PrismLandPlotLocation() {}
}