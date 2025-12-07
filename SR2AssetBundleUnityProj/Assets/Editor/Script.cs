using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Tools/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "../SR2EssentialsMod/Assets/";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
                                        BuildAssetBundleOptions.None, 
                                        EditorUserBuildSettings.activeBuildTarget);
        File.Delete(assetBundleDirectory+"srtwoessentials.assetbundle.manifest");
        File.Delete(assetBundleDirectory + "SR2EssentialsMod");
        File.Delete(assetBundleDirectory + "SR2EssentialsMod.manifest");
        File.Delete(assetBundleDirectory + "Assets");
        File.Delete(assetBundleDirectory + "Assets.manifest");
    }
}
