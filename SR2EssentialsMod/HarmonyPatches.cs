using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using UnityEngine.UI;

namespace SR2E.Patches;

[HarmonyPatch(typeof(WeaponVacuum), nameof(WeaponVacuum.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
internal class WeaponVacuumExpelPatch
{
    public static void Prefix(WeaponVacuum __instance, ref bool ignoreEmotions)
    {
        if (__instance._player.Ammo.GetSelectedEmotions() == null)
            ignoreEmotions = true;
    }
}
[HarmonyPatch(typeof(TeleporterNode), nameof(TeleporterNode.OnTriggerEnter))]
internal class TeleporterUsePatch
{
    public static void Prefix(Collider collider)
    {
        if (collider.gameObject == SceneContext.Instance.player)
            GameContext.Instance.AutoSaveDirector.SaveGame();
    }
}

/*
[HarmonyPatch(typeof(PauseMenuRoot), nameof(PauseMenuRoot.Awake))]
public static class SR2PauseMenuButtonPatch
{
    internal static List<CustomPauseMenuButton> buttons = new List<CustomPauseMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Postfix(PauseMenuRoot __instance)
    {
        if (safeLock) { return; }
        safeLock = true;
        VerticalLayoutGroup adapterView = __instance.gameObject.getObjRec<VerticalLayoutGroup>("AdapterView (Buttons)");
        Button exampleButton = __instance.gameObject.getObjRec<Button>("TextButton_Selectable_NoIcon(Clone)");
        foreach (CustomPauseMenuButton button in buttons)
        {
            if (button.name == null || button.label == null || button.action == null) continue;
            try
            {
                
                Button newButton = GameObject.Instantiate(exampleButton, adapterView.transform);
                newButton.transform.SetSiblingIndex(button.insertIndex);
                newButton.transform.GetChild(0).GetComponent<CanvasGroup>().enabled = false;
                newButton.transform.GetChild(0).GetChild(1).GetComponent<Il2CppTMPro.TextMeshProUGUI>().text = button.label;
                Object.Destroy(newButton.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>());
                Object.Destroy(newButton.transform.GetChild(0).GetChild(1).GetComponent<Il2CppMonomiPark.SlimeRancher.UI.Framework.Layout.GameObjectWatcher>());

                //Fix Selected Button SpaceBar Icon Dupe
                for (int i = 0; i < newButton.transform.GetChild(0).GetChild(0).GetChild(0).childCount; i++)
                    newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
                newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

                newButton.onClick = new Button.ButtonClickedEvent();
                newButton.onClick.AddListener((System.Action)(() =>
                {
                    button.action.Invoke();
                    //GameObject.Instantiate(button._prefabToSpawn);
                }));
                
            } catch (Exception e){Console.WriteLine(e);}
        }
        safeLock = false;
    }

}

*/






[HarmonyPatch(typeof(PauseMenuDirector), nameof(PauseMenuDirector.Start))]
public static class SR2PauseMenuButtonPatch
{
    internal static List<CustomPauseMenuButton> buttons = new List<CustomPauseMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Postfix(PauseMenuDirector __instance)
    {
        if (safeLock) { return; }
        safeLock = true;
        foreach (CustomPauseMenuButton button in buttons)
        {
            if (button.name == null || button.label == null ||  button.action == null) continue;
            try
            {
                if (button._model != null)
                {
                    if(__instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Contains(button._model))
                        continue;
                    if(!__instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Insert(button.insertIndex, button._model);
                    if(!__instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Insert(button.insertIndex, button._model);
                    if(!__instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Insert(button.insertIndex, button._model);
                    if(!__instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Insert(button.insertIndex, button._model);
                    if(!__instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Contains(button._model)) 
                        __instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Insert(button.insertIndex, button._model);
                    if(!__instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Insert(button.insertIndex, button._model);
                    continue;
                }
                CustomPauseItemModel model = ScriptableObject.CreateInstance<CustomPauseItemModel>();
                model.action = button.action;
                button._model = model;
                button._model.label = button.label;
                button._model.name = button.name;
                button._model.hideFlags |= HideFlags.HideAndDontSave;
                //button._model.prefabToSpawn = button._prefabToSpawn;
                
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Insert(button.insertIndex, button._model);
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Insert(button.insertIndex, button._model);
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Insert(button.insertIndex, button._model);
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Insert(button.insertIndex, button._model);
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Contains(button._model)) 
                    __instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Insert(button.insertIndex, button._model);
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Insert(button.insertIndex, button._model);
                if(!__instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreXboxSeriesAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreXboxSeriesAsset.items.Insert(button.insertIndex, button._model);
            } catch (Exception e){Console.Write(e); }
        }
        safeLock = false;
    }
/*
    private static void Postfix()
    {
        if (postSafeLock) return;
        postSafeLock = true;
        foreach (CustomPauseMenuButton button in buttons)
        {
            if(button.name==null||button.label==null||  button.action==null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "BButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent<CustomMainMenuButtonPressHandler>();
                    button._prefabToSpawn = obj;
                    button._definition.prefabToSpawn = obj;
                }
            } catch { }
        }
        postSafeLock = false;
    }*/
}


[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
public static class SR2MainMenuButtonPatch
{
    internal static List<CustomMainMenuButton> buttons = new List<CustomMainMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Prefix(MainMenuLandingRootUI __instance)
    {
        if (safeLock) { return; }
        safeLock = true;
        foreach (CustomMainMenuButton button in buttons)
        {
            if (button.name == null || button.label == null || button.icon == null || button.action == null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent<CustomMainMenuButtonPressHandler>();
                    button._prefabToSpawn = obj;
                }
                if (button._definition != null)
                {
                    if(__instance._continueGameConfig.items.Contains(button._definition))
                        continue;
                    __instance._continueGameConfig.items.Insert(button.insertIndex+1, button._definition);
                    __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex, button._definition);
                    __instance._newGameConfig.items.Insert(button.insertIndex, button._definition);
                    continue;
                }
                button._definition = ScriptableObject.CreateInstance<CreateNewUIItemDefinition>();
                button._definition.label = button.label;
                button._definition.name = button.name;
                button._definition.icon = button.icon;
                button._definition.hideFlags |= HideFlags.HideAndDontSave;
                button._definition.prefabToSpawn = button._prefabToSpawn;
                __instance._continueGameConfig.items.Insert(button.insertIndex + 1, button._definition);
                __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex , button._definition);
                __instance._newGameConfig.items.Insert(button.insertIndex, button._definition);
            } catch { }
        }
        safeLock = false;
    }

    private static void Postfix()
    {
        if (postSafeLock) return;
        postSafeLock = true;
        foreach (CustomMainMenuButton button in buttons)
        {
            if(button.name==null||button.label==null|| button.icon==null|| button.action==null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent<CustomMainMenuButtonPressHandler>();
                    button._prefabToSpawn = obj;
                    button._definition.prefabToSpawn = obj;
                }
            } catch { }
        }
        postSafeLock = false;
    }
}




[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadInitialSceneGroup))]
public static class SR2StartPatch
{
    public static bool Prefix()
    {
        if (SR2EEntryPoint.skipEngagementPrompt)
        {
            MelonLogger.Msg("Skipping engagement prompt!");

            var sl = SystemContext.Instance.SceneLoader;

            sl.LoadMainMenuSceneGroup();
            return false;
        }
        return true;
    }
}
[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.Awake))]
public static class SaveGameRootUIPatch
{
    public static void Prefix()
    {
        SR2EEntryPoint.SaveCountChanged = false;
    }
}