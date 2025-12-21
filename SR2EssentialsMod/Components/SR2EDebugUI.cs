using System;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.UI.Debug;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;

namespace SR2E;

[InjectClass]
internal class SR2EDebugUI : MonoBehaviour
{ 
	static GameObject player;
	internal static bool isEnabled;
	internal Font _helpFont;

	internal class DebugStatsManager
	{
		static bool playerDebugUIEnabled = false;
		private static SRCharacterController cc;
		private static SRCharacterInput ci;
		private static RegionMember rm;
		private static PlayerDebugHudUI playerDebugHudUI = null;
		private static bool accurate = false;

		internal static void TogglePlayerDebugUI()
		{
			if (playerDebugUIEnabled) DeActivatePlayerDebugUI();
			else ActivatePlayerDebugUI();
		}

		internal static void DeActivatePlayerDebugUI()
		{
			if (!playerDebugUIEnabled) return;
			if (playerDebugHudUI == null)
				playerDebugHudUI = Get<PlayerDebugHudUI>("PlayerDebug");
			if (playerDebugHudUI == null) return;
			if (playerDebugHudUI.gameObject.activeSelf)
				playerDebugHudUI.transform.gameObject.SetActive(false);
			playerDebugUIEnabled = false;
			accurate = RestoreDebugPlayerDebug.HasFlag();
			if (accurate)
			{
				rm.remove_RegionsChanged(OnRegionsChanged);
			}
		}

		internal static void ActivatePlayerDebugUI()
		{
			if (playerDebugUIEnabled) return;
			if (playerDebugHudUI == null) playerDebugHudUI = Get<PlayerDebugHudUI>("PlayerDebug");
			if (playerDebugHudUI == null) return;
			if (!playerDebugHudUI.gameObject.activeSelf)
				playerDebugHudUI.transform.gameObject.SetActive(true);
			player = Get<GameObject>("PlayerControllerKCC");
			
			if (player == null) return;
			cc = player.GetComponent<SRCharacterController>();
			ci = player.GetComponent<SRCharacterInput>();
			rm = player.GetComponent<RegionMember>();
			for (int i = 0; i < playerDebugHudUI.transform.childCount; i++)
			{
				TMP_Text tmpText = playerDebugHudUI.transform.GetChild(i).GetComponent<TMP_Text>();
				if (tmpText != null)
					tmpText.color = new Color(1, 1, 1, 1);
			}

			accurate = RestoreDebugPlayerDebug.HasFlag();
			playerDebugUIEnabled = true;
			if (accurate)
			{
				rm.add_RegionsChanged(OnRegionsChanged);
			}
		}

		internal static RegionMember.MembershipChanged OnRegionsChanged = 
			(RegionMember.MembershipChanged)((sender, args) =>
			{
				MelonLogger.Msg("On Change");
				Il2CppSystem.Collections.Generic.List<Region> regions = sender;
				foreach (var re in regions)
				{
					MelonLogger.Msg("sender: "+re.name);
				}
				Il2CppSystem.Collections.Generic.List<Region> regions2 = args;
				foreach (var re in regions2)
				{
					MelonLogger.Msg("args"+re.name);
				}
				if(!playerDebugUIEnabled) return;
				playerDebugHudUI._cell.SetText("Cell: ");
		});
        internal static void Update()
        {
            if(!playerDebugUIEnabled) return;
            if(playerDebugHudUI==null)
            { playerDebugUIEnabled = false; return; }
            if(cc==null)
            { playerDebugUIEnabled = false; return; }

            if (accurate)
            {
	            //I set the texts with multiple values to max decimal 3, cuz I can't image it being longer
	            
	            //I guess its vertical, if the other one is horizontal? 
	            playerDebugHudUI._velocity.SetText("Vertical Velocity: "+cc.Velocity.y);
	            //What is horizontal even? X and Z combined?
	            playerDebugHudUI._horizontalVelocity.SetText($"Horizontal Velocity: {cc.Velocity.x+cc.Velocity.z}");
	            playerDebugHudUI._slopeText.SetText($"Slope: {cc.CurrentSlopeAngle}");
	            playerDebugHudUI._playerLocation.SetText($"Location: ({Math.Round(cc.Position.x,3)} {Math.Round(cc.Position.y,3)} {Math.Round(cc.Position.z,3)})");
	            playerDebugHudUI._cell.SetText($"Cell: {"What the hell is that?"}");
	            playerDebugHudUI._lookInput.SetText($"Look Input: ({Math.Round(ci.LookInput.x,3)} {Math.Round(ci.LookInput.y,3)})");
	            //I'm just guessing at this point?
	            string abilityText = "";
	            foreach (var ability in cc.AbilityBehaviors)
	            {
		            if (!ability.IsActive) continue;
		            if (!string.IsNullOrWhiteSpace(abilityText)) abilityText += ", ";
		            abilityText += ability.GetType().Name.Replace("AbilityBehaviour", "");
	            }
	            playerDebugHudUI._activeAbilities.SetText($"Active Abilities: "+abilityText);
            }
            else
            {
	            playerDebugHudUI._velocity.SetText($"FPS: {(int)(1f / Time.unscaledDeltaTime)}");
	            playerDebugHudUI._horizontalVelocity.SetText($"Position: {cc.Position.x} {cc.Position.y} {cc.Position.z}");
	            playerDebugHudUI._slopeText.SetText($"Rotation: {player.transform.eulerAngles.y}");
	            playerDebugHudUI._playerLocation.SetText($"Velocity: {cc.Velocity.x} {cc.Velocity.y} {cc.Velocity.z}");
	            playerDebugHudUI._cell.SetText($"InputVector: {cc.InputVector.x} {cc.InputVector.y}");
	            playerDebugHudUI._lookInput.SetText($"LookInput: {cc.LookVector.x} {cc.LookVector.y} {cc.LookVector.z}");
	            playerDebugHudUI._activeAbilities.SetText($"Slope: {cc.CurrentSlopeAngle}");
            }
        }
    }
	private void Awake()
	{
		isEnabled = SR2EEntryPoint.enableDebugDirector;
		_helpFont = Font.CreateDynamicFontFromOSFont("Consolas", 18);
	}

	private void Update()
	{
		if (!isEnabled) return;

		if (MenuEUtil.isAnyMenuOpen) return;
		if (MenuEUtil.isAnyPopUpOpen) return;
		if (Time.timeScale == 0)  return;
		if (!inGame) return;
		if (SR2EWarpManager.warpTo != null) return;
		switch (systemContext.SceneLoader.CurrentSceneGroup.name) { case "StandaloneStart": case "CompanyLogo": case "LoadScene": return; }
		if (LKey.Alpha0.OnKeyDown()) SR2ECommandManager.ExecuteByString("upgrade set * 10", true);
		if (LKey.Alpha7.OnKeyDown()) SR2ECommandManager.ExecuteByString("infenergy true", true);
		if (LKey.Alpha8.OnKeyDown()) SR2ECommandManager.ExecuteByString("infhealth", true);
		if (LKey.Alpha9.OnKeyDown()) autoSaveDirector.SaveAllNow();
		if (LKey.P.OnKeyDown()) SR2ECommandManager.ExecuteByString("pedia unlock * false", true);
		if (LKey.K.OnKeyDown()) SR2ECommandManager.ExecuteByString("clearinv", true);
		if (LKey.L.OnKeyDown()) SR2ECommandManager.ExecuteByString("refillinv", true);
		if (LKey.N.OnKeyDown()) SR2ECommandManager.ExecuteByString("noclip", true);
		if (LKey.U.OnKeyDown()) GUIUtility.systemCopyBuffer = Warp.CurrentLocation().ToString();
		if (LKey.J.OnKeyDown()) try { Warp.FromString(GUIUtility.systemCopyBuffer.Trim()).WarpPlayerThere(); } catch {}
		if (LKey.KeypadPlus.OnKeyDown()) SR2ECommandManager.ExecuteByString("newbucks 1000", true);
		if (LKey.KeypadMinus.OnKeyDown()) SR2ECommandManager.ExecuteByString("newbucks -1000", true);
		if (LKey.LeftBracket.OnKeyDown()) SR2ECommandManager.ExecuteByString("fastforward -1", true);
		if (LKey.RightBracket.OnKeyDown()) SR2ECommandManager.ExecuteByString("fastforward 1", true);
		if (ExperimentalKeyCodes.HasFlag())
		{
		}
	}
	private void OnGUI()
	{
		if (isEnabled)
		{
			GUI.skin.label.font = _helpFont;
			GUI.skin.label.alignment = TextAnchor.UpperRight;
			string text = "<b>DEBUG MODE INFO" + 
							"\n\nGIVE ALL PERSONAL UPGRADES     0 " +
							"\nGIVE ALL PEDIA ENTRIES     P " + 
							"\nTOGGLE INFINITE ENERGY     7 " +
							"\nTOGGLE INFINITE HEALTH     8 " +
							"\nFORCE SAVE     9 " +
							"\n\nCLEAR INVENTORY     K " +
							"\nREFILL INVENTORY     L " +
							"\nTOGGLE NOCLIP     N " +
							"\n\nSAVE LOCATION     U " +
							"\nGOTO LOCATION     J " +
							"\n\nADD 1000 CREDITS     KeyPad+ " +
							"\nREMOVE 1000 CREDITS     KeyPad- " +
							"\nDECREMENT TIME OF DAY     [ " +
							"\nINCREMENT TIME OF DAY     ] </b>";
			for (int i = -2; i <= 2; i += 2)
				for (int j = -2; j <= 2; j += 2)
				{
					GUI.skin.label.normal.textColor = Color.black;
					GUI.Label(new Rect((float)i, (float)j, (float)Screen.width, (float)Screen.height), text);
				}
			GUI.skin.label.normal.textColor = Color.white;
			GUI.Label(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), text);
		}
	}

}
