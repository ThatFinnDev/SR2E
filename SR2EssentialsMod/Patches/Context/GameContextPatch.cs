using System;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.DebugTool;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Buttons;
using SR2E.Buttons.OptionsUI;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Enums.Sounds;
using SR2E.Managers;
using SR2E.Menus;
using SR2E.Patches.General;
using SR2E.Patches.Options;
using SR2E.Popups;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(GameContext), nameof(GameContext.Start))]
internal class GameContextPatch
{
    internal static CustomPauseMenuButton cheatMenuButton;
    internal static void Postfix(GameContext __instance)
    {
        var damageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>();
        damageSource._logMessage = "Modded.SR2EKill";
        damageSource.name = "ModdedSR2EDamage";
        damageSource.hideFlags |= HideFlags.HideAndDontSave;
         _killDamage = new Damage
        {
            Amount = 99999999, DamageSource = damageSource,
        };
        OptionsUIRootApplyPatch.Postfix();
        foreach (ParticleSystemRenderer particle in Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>())
        {
            var pname = particle.gameObject.name.Replace(' ', '_');
            if (!LookupEUtil.FXLibrary.ContainsKey(particle.gameObject))
                LookupEUtil.FXLibrary.AddItems(particle.gameObject, particle, pname);
            if (!LookupEUtil.FXLibraryReversable.ContainsKey(pname))
                LookupEUtil.FXLibraryReversable.AddItems(pname, particle, particle.gameObject);
        }


        if (!SR2EEntryPoint.addedButtons)
        {
            SR2EEntryPoint.addedButtons = true;
            if (AddModMenuButton.HasFlag())
            {
                LocalizedString label = AddTranslationFromSR2E("buttons.mods.label", "b.button_mods_sr2e", "UI");
                new CustomMainMenuButton(label, EmbeddedResourceEUtil.LoadSprite("Assets.modsMenuIcon.png").CopyWithoutMipmaps(), 4, (System.Action)(() => { MenuEUtil.GetMenu<SR2EModMenu>().Open(); }));
                new CustomPauseMenuButton(label, 3, (System.Action)(() => { MenuEUtil.GetMenu<SR2EModMenu>().Open(); }));
                
            }
            if (AddMockMainMenuButtons.HasFlag())
            {
                var con = new CustomMainMenuContainerButton(AddTranslation("SubMenu"), null, 3, null);
                con.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubmenu1"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the submenu1"); })));
                con.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubmenu2"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the submenu2"); })));
                con.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubmenu3"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the submenu3"); })));
                con.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubmenu4"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the submenu4"); })));
                con.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubmenu5"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the submenu5"); })));
                con.AddSubButton(new CustomMainMenuButton(AddTranslation("InBoth"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in both"); })), false);
                // Sub Sub menus dont work :(
                //var three = new CustomMainMenuButton(AddTranslation("InAllThree"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("InAllThree"); }));
                //con.AddSubButton(three,false);
                //var subsub = new CustomMainMenuContainerButton(AddTranslation("SubSubMenu"), null, 0, null);
                //con.AddSubButton(subsub);
                //subsub.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubSubmenu1"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the subsubmenu1"); })));
                //subsub.AddSubButton(new CustomMainMenuButton(AddTranslation("InSubSubmenu2"), null, 0, (System.Action)(() => { SR2ETextViewer.Open("This is a button in the subsubmenu2"); })));
                //subsub.AddSubButton(three,false);

            }
            if (AddCheatMenuButton.HasFlag()) cheatMenuButton = new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.cheatmenu.label", "b.button_cheatmenu_sr2e", "UI"), 4, (System.Action)(() => { MenuEUtil.GetMenu<SR2ECheatMenu>().Open(); }));
            if (DevMode.HasFlag()||RestoreDebugPlayerDebug.HasFlag()) new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.debugplayer.label", "b.debug_player_sr2e", "UI"), 3, (System.Action)(() => { SR2EDebugUI.DebugStatsManager.TogglePlayerDebugUI(); }));
            if (AddMockOptionsUIButtons.HasFlag())
            {
                var testCategory1 = new CustomOptionsUICategory(AddTranslation("AllTheTime"), 4,null, OptionsCategoryVisibleState.AllTheTime);
                var testCategory2 = new CustomOptionsUICategory(AddTranslation("MainOnly"), 4,null, OptionsCategoryVisibleState.MainMenuOnly);
                var testCategory3 = new CustomOptionsUICategory(AddTranslation("InGameOnly"), 4,null, OptionsCategoryVisibleState.InGameOnly);
                testCategory1.AddButton(new CustomOptionsButtonValues
                (AddTranslation("GlobalTest"),AddTranslation("This is an example description"),
                    "sr2e.mock.global1",1,true,false,false, ((value) =>
                    { 
                        MelonLogger.Msg("It has been changed to "+value);
                    }), OptionsButtonType.OptionsUI,
                    AddTranslation("Value0"),AddTranslation("Value1"),AddTranslation("Value2"),AddTranslation("Value3")
                    ));
                testCategory2.AddButton(new CustomOptionsButtonValues
                (AddTranslation("MainMenuOnlyTest"),AddTranslation("This wraps around!"),
                    "sr2e.mock.mainmenuonly1",1,true,true,false, ((value) =>
                    { 
                        MelonLogger.Msg("It has been changed to "+value);
                    }), OptionsButtonType.OptionsUI,
                    AddTranslation("G0"),AddTranslation("G1"),AddTranslation("G2"),AddTranslation("G3")
                ));
                testCategory3.AddButton(new CustomOptionsButtonValues
                (AddTranslation("InGameOnlyTest"),AddTranslation("This doesn't apply immediately!"),
                    "sr2e.mock.ingameonly1",1,false,false,false, ((value) =>
                    { 
                        MelonLogger.Msg("It has been changed to "+value);
                    }), OptionsButtonType.InGameOptionsUIOnly,
                    AddTranslation("Value0"),AddTranslation("Value1"),AddTranslation("Value2"),AddTranslation("Value3")
                ));
                var coolButton = new CustomOptionsButtonValues
                (AddTranslation("MultiCategoryTest"), AddTranslation("This is in multiple categories! And shouldnt be 2 times in MainMenuOnly!"),
                    "sr2e.mock.multitest", 1, true, false, false, ((value) =>
                    {
                        MelonLogger.Msg("It has been changed to " + value);
                    }), OptionsButtonType.OptionsUI,
                    AddTranslation("Value0"), AddTranslation("Value1"), AddTranslation("Value2"),
                    AddTranslation("Value3")
                );
                testCategory1.AddButton(coolButton);
                testCategory2.AddButton(coolButton);
                testCategory2.AddButton(coolButton);
                testCategory3.AddButton(coolButton);
            }
        }

        Time.timeScale = 1f;
        try
        {
            LookupEUtil.closeInput = Get<InputEvent>("Close");
            if(LookupEUtil.closeInput != null)
                LookupEUtil.closeInput.add_Performed((System.Action<InputEventData>)((data) =>
                {
                    var menu = MenuEUtil.GetOpenMenu();
                    if(menu!=null) menu.OnCloseUIPressed();
                }));
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

        if(RestoreDebugFPSViewer.HasFlag()) foreach (var display in GetAllInScene<FPSDisplay>())
            display.AddComponent<FPSDisplayFixer>();
        if (RestoreDebugDebugUI.HasFlag()) __instance.AddComponent<DebugDirectorFixer>();
        foreach (var expansion in SR2EEntryPoint.expansionsV1V2)
            try { expansion.OnGameContext(__instance); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.AfterGameContext(__instance); }
            catch (Exception e) { MelonLogger.Error(e); }
        SR2ECallEventManager.ExecuteWithArgs(CallEvent.AfterGameContextLoad, ("gameContext", __instance));
        foreach (var pair in SR2EEntryPoint.menus)
            try { pair.Key.OnGameContext(__instance); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var pair in SR2ECommandManager.commands)
            try { pair.Value.OnGameContext(__instance); }
            catch (Exception e) { MelonLogger.Error(e); }
    }


}