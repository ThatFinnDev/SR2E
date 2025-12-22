using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace SR2E.Patches.General;


[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
internal class TeleportPlayerPatch
{
    static bool IsValidForRuntimeLoad(AssetReference assetReference) => assetReference != null && assetReference.RuntimeKeyIsValid();
    
    internal static bool isTeleportingPlayer = false;
    public static void Prefix(SceneLoader __instance, ref SceneGroup sceneGroup, ref SceneLoadingParameters parameters)
    {
        if (isTeleportingPlayer)
        {
            isTeleportingPlayer = false;
            if (parameters != null) parameters.ReloadAllCoreScenes = false;
            if (parameters == null) parameters = new SceneLoadingParameters();
            var cleanedSceneReferences = new List<AssetReference>();
            var oldReferences = sceneGroup._sceneReferences.ToNetList();
            foreach (var reference in oldReferences)
                if(IsValidForRuntimeLoad(reference))
                    cleanedSceneReferences.Add(reference);
            sceneGroup._sceneReferences = cleanedSceneReferences.ToArray();
            var oldCore = sceneGroup._coreSceneReference;
            sceneGroup._coreSceneReference = __instance.DefaultGameplaySceneGroup._coreSceneReference;
            var sG = sceneGroup;
            parameters.OnSceneGroupLoadedPhase2 += new System.Action<Il2CppSystem.Action<SceneLoadErrorData>>(action =>
            {
                sG._coreSceneReference = oldCore;
                sG._sceneReferences = oldReferences.ToArray();
            });

        }
    } 
}