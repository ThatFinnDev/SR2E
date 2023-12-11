using System.IO;
using System.Reflection;
using Il2CppInterop.Runtime;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using UnityEngine.Localization;

namespace SR2E;

[HarmonyPatch(typeof(WeaponVacuum), nameof(WeaponVacuum.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
internal class WeaponVacuumExpelPatch
{
    public static void Prefix(WeaponVacuum __instance, ref bool ignoreEmotions)
    {
        if (__instance._player.Ammo.GetSelectedEmotions() == null)
        {
            ignoreEmotions = true;
        }
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

public class CustomMainMenuButton
{
    public string name;
    public LocalizedString label;
    public Sprite icon;
    public int insertIndex;
    public Il2CppSystem.Type monoBehaviourToActivate;
    internal GameObject _prefabToSpawn;
    internal CreateNewUIItemDefinition _definition;

    public CustomMainMenuButton(string name, LocalizedString label, Sprite icon, int insertIndex, System.Type monoBehaviourToActivate)
    {
        this.name = name;
        this.label = label;
        this.icon = icon;
        this.insertIndex = insertIndex;
        this.monoBehaviourToActivate = Il2CppType.From(monoBehaviourToActivate);
    }
}
[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
public static class SR2ModMenuButtonPatch
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
            if (button.name == null || button.label == null || button.icon == null || button.monoBehaviourToActivate == null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent(button.monoBehaviourToActivate);
                    button._prefabToSpawn = obj;
                }
                if (button._definition != null)
                {
                    foreach (var i in __instance._newGameConfig.items) if (i.name == button.name) continue;
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
                __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex + 2, button._definition);
                __instance._newGameConfig.items.Insert(button.insertIndex + 2, button._definition);
            } catch { }
        }
    }

    private static void Postfix()
    {
        if (postSafeLock) return;
        postSafeLock = true;
        foreach (CustomMainMenuButton button in buttons)
        {
            if(button.name==null||button.label==null|| button.icon==null|| button.monoBehaviourToActivate==null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent(button.monoBehaviourToActivate);
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