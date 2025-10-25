using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.Audio;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using SR2E.Buttons;
using SR2E.Enums.Sounds;
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
        _killDamage = new Damage
        {
            Amount = 99999999, DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>(),
        };
        _killDamage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
        
        foreach (ParticleSystemRenderer particle in Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>())
        {
            var pname = particle.gameObject.name.Replace(' ', '_');
            if (!LookupEUtil.FXLibrary.ContainsKey(particle.gameObject))
                LookupEUtil.FXLibrary.AddItems(particle.gameObject, particle, pname);
            if (!LookupEUtil.FXLibraryReversable.ContainsKey(pname))
                LookupEUtil.FXLibraryReversable.AddItems(pname, particle, particle.gameObject);
        }

        LookupEUtil.vaccableGroup = Get<IdentifiableTypeGroup>("VaccableNonLiquids");

        foreach (KeyValuePair<string, string> pair in teleportersToAdd)
            AddTeleporter(pair.Key, pair.Value);

        if (!SR2EEntryPoint.addedButtons)
        {
            SR2EEntryPoint.addedButtons = true;
            if (AddModMenuButton.HasFlag())
            {
                LocalizedString label = AddTranslationFromSR2E("buttons.mods.label", "b.button_mods_sr2e", "UI");
                new CustomMainMenuButton(label, EmbeddedResourceEUtil.LoadSprite("Assets.modsMenuIcon.png"), 4, (System.Action)(() => { MenuEUtil.GetMenu<SR2EModMenu>().Open(); }));
                new CustomPauseMenuButton(label, 3, (System.Action)(() => { MenuEUtil.GetMenu<SR2EModMenu>().Open(); }));
            }

            if (AddCheatMenuButton.HasFlag()) new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.cheatmenu.label", "b.button_cheatmenu_sr2e", "UI"), 4, (System.Action)(() => { MenuEUtil.GetMenu<SR2ECheatMenu>().Open(); }));
            if (DevMode.HasFlag()) new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.debugplayer.label", "b.debug_player_sr2e", "UI"), 3, (System.Action)(() => { SR2EDebugUI.DebugStatsManager.TogglePlayerDebugUI(); }));

        }

        Time.timeScale = 1f;
        try
        {
            LookupEUtil.actionMaps = new Dictionary<string, InputActionMap>();
            LookupEUtil.MainGameActions = new Dictionary<string, InputAction>();
            LookupEUtil.PausedActions = new Dictionary<string, InputAction>();
            LookupEUtil.DebugActions = new Dictionary<string, InputAction>();
            foreach (InputActionMap map in gameContext.InputDirector._inputActions.actionMaps)
                LookupEUtil.actionMaps.Add(map.name, map);
            foreach (InputAction action in LookupEUtil.actionMaps["MainGame"].actions)
                LookupEUtil.MainGameActions.Add(action.name, action);
            foreach (InputAction action in LookupEUtil.actionMaps["Paused"].actions)
                LookupEUtil.PausedActions.Add(action.name, action);
            foreach (InputAction action in LookupEUtil.actionMaps["Debug"].actions)
                LookupEUtil.DebugActions.Add(action.name, action);
        }
        catch (Exception e)
        {
            MelonLogger.Error(e);
            MelonLogger.Error("There was a problem loading SR2 action maps!");
        }

        try
        {
            AudioEUtil._menuSounds.Add(MenuSound.Click,Get<SECTR_AudioCue>("Click1"));
            AudioEUtil._menuSounds.Add(MenuSound.SelectCategory,Get<SECTR_AudioCue>("Click2"));
            AudioEUtil._menuSounds.Add(MenuSound.Apply,Get<SECTR_AudioCue>("Click3"));
            AudioEUtil._menuSounds.Add(MenuSound.Pop,Get<SECTR_AudioCue>("Click4"));
            AudioEUtil._menuSounds.Add(MenuSound.ButtonFocused,Get<SECTR_AudioCue>("Click5"));
            AudioEUtil._menuSounds.Add(MenuSound.Hover,Get<SECTR_AudioCue>("ClickRollover"));
            AudioEUtil._menuSounds.Add(MenuSound.Error,Get<SECTR_AudioCue>("ClickError"));
            AudioEUtil._menuSounds.Add(MenuSound.OpenMenu,Get<SECTR_AudioCue>("UIOpen"));
            AudioEUtil._menuSounds.Add(MenuSound.CloseMenu,Get<SECTR_AudioCue>("UIClose"));
            AudioEUtil._menuSounds.Add(MenuSound.OpenPopup,Get<SECTR_AudioCue>("UIOpen2"));
            AudioEUtil._menuSounds.Add(MenuSound.ClosePopup,Get<SECTR_AudioCue>("UIClose2"));
            //AudioEUtil._defaultMenuSounds = Get<UIAudioTable>("DefaultMenuSounds");
        }
        catch (Exception e)
        {
            MelonLogger.Error(e);
            MelonLogger.Error("There was a problem loading sounds!");
        }
        //
        foreach (var expansion in SR2EEntryPoint.expansionsAll)
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
        StaticTeleporterNode teleporter = GameObject.Instantiate(LookupEUtil.GetGadgetDefinitionByName(gadgetName).prefab.transform.GetObjectRecursively<GadgetTeleporterNode>("Teleport Collider").gameObject.GetComponent<StaticTeleporterNode>());
        teleporter.gameObject.SetActive(false); teleporter.name = "TP-"+sceneGroup; teleporter.gameObject.MakePrefab(); teleporter.gameObject.MakePrefab(); teleporter._hasDestination = true;
        SR2EWarpManager.teleporters.TryAdd(sceneGroup, teleporter);
    }

}