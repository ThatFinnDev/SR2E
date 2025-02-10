using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using SR2E.Buttons;
using SR2E.Managers;
using SR2E.Menus;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(GameContext), nameof(GameContext.Start))]
internal class GameContextPatch
{
    internal static void Postfix(GameContext __instance)
    {
        SR2EUtils._killDamage = new Damage
        {
            Amount = 99999999, DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>(),
        };
        SR2EUtils._killDamage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
        AutoSaveDirector autoSaveDirector = GameContext.Instance.AutoSaveDirector;
        autoSaveDirector.saveSlotCount = SAVESLOT_COUNT.Get();

        foreach (ParticleSystemRenderer particle in Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>())
        {
            var pname = particle.gameObject.name.Replace(' ', '_');
            if (!FXLibrary.ContainsKey(particle.gameObject))
                FXLibrary.AddItems(particle.gameObject, particle, pname);
            if (!FXLibraryReversable.ContainsKey(pname))
                FXLibraryReversable.AddItems(pname, particle, particle.gameObject);
        }

        vaccableGroup = Get<IdentifiableTypeGroup>("VaccableNonLiquids");

        foreach (KeyValuePair<string, string> pair in teleportersToAdd)
            AddTeleporter(pair.Key, pair.Value);

        if (!SR2EEntryPoint.addedButtons)
        {
            SR2EEntryPoint.addedButtons = true;
            if (AddModMenuButton.HasFlag())
            {
                LocalizedString label = AddTranslationFromSR2E("buttons.mods.label", "b.button_mods_sr2e", "UI");
                new CustomMainMenuButton(label, LoadSprite("modsMenuIcon"), 2, (System.Action)(() => { GM<SR2EModMenu>().Open(); }));
                new CustomPauseMenuButton(label, 3, (System.Action)(() => { GM<SR2EModMenu>().Open(); }));
            }

            if (!SR2EEntryPoint.cheatsEnabledOnSave)
                if (AddCheatMenuButton.HasFlag())
                {
                    SR2EEntryPoint.cheatMenuButton = new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.cheatmenu.label", "b.button_cheatmenu_sr2e", "UI"), 4, (System.Action)(() => { GM<SR2ECheatMenu>().Open(); }));
                    if (!SR2EEntryPoint.enableCheatMenuButton) SR2EEntryPoint.cheatMenuButton.Remove();
                }
            if (DevMode.HasFlag()) new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.debugplayer.label", "b.debug_player_sr2e", "UI"), 3, (System.Action)(() => { SR2EDebugDirector.DebugStatsManager.TogglePlayerDebugUI(); }));

        }

        Time.timeScale = 1f;
        try
        {
            actionMaps = new Dictionary<string, InputActionMap>();
            MainGameActions = new Dictionary<string, InputAction>();
            PausedActions = new Dictionary<string, InputAction>();
            DebugActions = new Dictionary<string, InputAction>();
            foreach (InputActionMap map in GameContext.Instance.InputDirector._inputActions.actionMaps)
                actionMaps.Add(map.name, map);
            foreach (InputAction action in actionMaps["MainGame"].actions)
                MainGameActions.Add(action.name, action);
            foreach (InputAction action in actionMaps["Paused"].actions)
                PausedActions.Add(action.name, action);
            foreach (InputAction action in actionMaps["Debug"].actions)
                DebugActions.Add(action.name, action);
        }
        catch (Exception e)
        {
            MelonLogger.Error(e);
            MelonLogger.Error("There was a problem loading SR2 action maps!");
        }

        //
        foreach (var expansion in SR2EEntryPoint.expansions)
            try
            {
                expansion.OnGameContext(__instance);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        foreach (var pair in SR2ECommandManager.commands)
            try
            {
                pair.Value.OnGameContext(__instance);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
    }

    internal static Dictionary<string,string> teleportersToAdd = new Dictionary<string, string>()
    {
        {"SceneGroup.ConservatoryFields", "TeleporterHomeBlue"},
        {"SceneGroup.RumblingGorge", "TeleporterZoneGorge"},
        {"SceneGroup.LuminousStrand", "TeleporterZoneStrand"},
        {"SceneGroup.PowderfallBluffs", "TeleporterZoneBluffs"},
        {"SceneGroup.Labyrinth", "TeleporterZoneLabyrinth"},
    };
    internal static void AddTeleporter(string sceneGroup, string gadgetName)
    {
        StaticTeleporterNode teleporter = GameObject.Instantiate(getGadgetDefByName(gadgetName).prefab.transform.getObjRec<GadgetTeleporterNode>("Teleport Collider").gameObject.GetComponent<StaticTeleporterNode>());
        teleporter.gameObject.SetActive(false); teleporter.name = "TP-"+sceneGroup; teleporter.gameObject.MakePrefab(); teleporter.gameObject.MakePrefab(); teleporter._hasDestination = true;
        SR2EWarpManager.teleporters.TryAdd(sceneGroup, teleporter);
    }

}