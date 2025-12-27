
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SR2E.Patches.General;


[HarmonyPatch(typeof(SceneLoader))]
internal class SceneLoaderLoadSceneGroupPatch
{
    static bool IsValidForRuntimeLoad(AssetReference assetReference) => assetReference != null && assetReference.RuntimeKeyIsValid();
    
    internal static bool isTeleportingPlayer = false;
    [HarmonyPrefix,HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
    public static void LoadSceneGroup_Prefix(SceneLoader __instance, ref SceneGroup sceneGroup, ref SceneLoadingParameters parameters)
    {
        if (isTeleportingPlayer)
        {
            if (parameters != null) parameters.ReloadAllCoreScenes = false;
        }
        if (!isTeleportingPlayer && !TryFixingInvalidSceneGroups.HasFlag()) return;
        try
        {
            bool hasInvalidSceneReferences = false;
            var cleanedSceneReferences = new List<AssetReference>();
            var oldReferences = sceneGroup._sceneReferences.ToNetList();
            foreach (var reference in oldReferences)
            {
                if (IsValidForRuntimeLoad(reference)) cleanedSceneReferences.Add(reference);
                else hasInvalidSceneReferences = true;
            }

            if (hasInvalidSceneReferences)
                sceneGroup._sceneReferences = cleanedSceneReferences.ToArray();
            var oldCore = sceneGroup._coreSceneReference;
            if (!IsValidForRuntimeLoad(sceneGroup._coreSceneReference))
            {
                hasInvalidSceneReferences = true;
                if (sceneGroup.IsGameplay)
                    sceneGroup._coreSceneReference = __instance.DefaultGameplaySceneGroup._coreSceneReference;
                else sceneGroup._coreSceneReference = __instance.MainMenuSceneGroup._coreSceneReference;
            }

            var sG = sceneGroup;
            if (parameters == null && hasInvalidSceneReferences) parameters = new SceneLoadingParameters();
            if (hasInvalidSceneReferences)
                parameters.OnSceneGroupLoadedPhase2 +=
                    new System.Action<Il2CppSystem.Action<SceneLoadErrorData>>(action =>
                    {
                        sG._coreSceneReference = oldCore;
                        sG._sceneReferences = oldReferences.ToArray();
                    });
            if (DebugLogging.HasFlag())
            {
                if (hasInvalidSceneReferences)
                    MelonLogger.Msg("HadInvalidSceneReferences");
            }
        }
        catch (Exception e)
        {
            MelonLogger.Msg(e);
        }
        isTeleportingPlayer = false;
    } 
    
    

}