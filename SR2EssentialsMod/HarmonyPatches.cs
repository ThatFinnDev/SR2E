using System.IO;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;

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
[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
public static class SR2ModMenuButtonPatch
{
    public static CreateNewUIItemDefinition mmButton = null;
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static GameObject modMenuButtonPrefab = null;
    public static void Prefix(MainMenuLandingRootUI __instance)
    {
            if (safeLock) { return; }
            safeLock = true;
            if (modMenuButtonPrefab == null)
            {
                var obj = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(obj);
                obj.name = "ModMenuButtonStarter";
                obj.transform.parent = rootOBJ.transform;
                obj.AddComponent<ModMenuActivator>();
                obj.AddComponent<ModMenuActivator>().enabled=true;
                modMenuButtonPrefab = obj;
            }

            if (mmButton != null)
            {
                foreach (var i in __instance._newGameConfig.items)
                    if (i.name == "ModMenu")
                        return;
                var bbd2 = mmButton;
                __instance._continueGameConfig.items.Insert(3, bbd2);
                __instance._existingGameNoContinueConfig.items.Insert(2, bbd2);
                __instance._newGameConfig.items.Insert(2, bbd2);
                return;
            }
            mmButton = ScriptableObject.CreateInstance<CreateNewUIItemDefinition>();
            
            mmButton.label = AddTranslation("Mods", "b.buttonMods", "UI");
            
            
            mmButton.name = "ModMenu";
            mmButton.icon = LibraryUtils.ConvertToSprite(LoadImage("modsMenuIcon"));
            mmButton.hideFlags |= HideFlags.HideAndDontSave;
            mmButton.prefabToSpawn = modMenuButtonPrefab;
            __instance._continueGameConfig.items.Insert(3,mmButton);
            __instance._existingGameNoContinueConfig.items.Insert(2,mmButton);
            __instance._newGameConfig.items.Insert(2,mmButton);
            
        }

    private static void Postfix()
    {
        if (postSafeLock) return;
        postSafeLock = true;
        if (modMenuButtonPrefab!=null)
            mmButton.prefabToSpawn = modMenuButtonPrefab;
        postSafeLock = false;
    }

    public static Texture2D LoadImage(string filename)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
        byte[] array = new byte[manifestResourceStream.Length];
        manifestResourceStream.Read(array, 0, array.Length);
        Texture2D texture2D = new Texture2D(1, 1);
        ImageConversion.LoadImage(texture2D, array);
        texture2D.filterMode = FilterMode.Bilinear;
        return texture2D;
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