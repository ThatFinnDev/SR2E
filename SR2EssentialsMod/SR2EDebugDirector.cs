using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI.Debug;
using Il2CppTMPro;
using SR2E.Managers;
using UnityEngine.UI;
using Key = SR2E.Enums.Key;

namespace SR2E;

[RegisterTypeInIl2Cpp(false)]
internal class SR2EDebugDirector : MonoBehaviour
{
	internal static bool isEnabled;
	internal Font _helpFont;
	internal class DebugStatsManager
    {
	    static bool playerDebugUIEnabled = false;
        private static SRCharacterController cc;
        private static PlayerDebugHudUI playerDebugHudUI = null;

        internal static void TogglePlayerDebugUI()
        {
            if (playerDebugUIEnabled) DeActivatePlayerDebugUI();
            else ActivatePlayerDebugUI();
        }
        internal static void DeActivatePlayerDebugUI()
        {
            if(!playerDebugUIEnabled) return;
            if(playerDebugHudUI==null) 
                playerDebugHudUI = Get<PlayerDebugHudUI>("PlayerDebug");
            if(playerDebugHudUI==null) return;
            if(playerDebugHudUI.gameObject.activeSelf)
                playerDebugHudUI.transform.gameObject.SetActive(false);
            playerDebugUIEnabled = false;
        }
        internal static void ActivatePlayerDebugUI()
        {
            if(playerDebugUIEnabled) return;
            if(playerDebugHudUI==null) playerDebugHudUI = Get<PlayerDebugHudUI>("PlayerDebug");
            if(playerDebugHudUI==null) return;
            if(!playerDebugHudUI.gameObject.activeSelf)
                playerDebugHudUI.transform.gameObject.SetActive(true);
            if(player==null) return;
            cc = player.GetComponent<SRCharacterController>();
            for (int i = 0; i < playerDebugHudUI.transform.childCount; i++)
            {
                TMP_Text tmpText = playerDebugHudUI.transform.GetChild(i).GetComponent<TMP_Text>();
                if (tmpText != null)
                    tmpText.color = new Color(1, 1, 1, 1);
            }
            playerDebugUIEnabled = true;
        }
        internal static void Update()
        {
            if(!playerDebugUIEnabled) return;
            if(playerDebugHudUI==null)
            { playerDebugUIEnabled = false; return; }
            if(cc==null)
            { playerDebugUIEnabled = false; return; }
            
            playerDebugHudUI._velocity.SetText($"FPS: {(int)(1f / Time.unscaledDeltaTime)}");
            playerDebugHudUI._horizontalVelocity.SetText($"Position: {cc.Position.x} {cc.Position.y} {cc.Position.z}");
            playerDebugHudUI._slopeText.SetText($"Rotation: {player.transform.eulerAngles.y}");
            playerDebugHudUI._playerLocation.SetText($"Velocity: {cc.Velocity.x} {cc.Velocity.y} {cc.Velocity.z}");
            playerDebugHudUI._cell.SetText($"InputVector: {cc.InputVector.x} {cc.InputVector.y}");
            playerDebugHudUI._lookInput.SetText($"LookInput: {cc.LookVector.x} {cc.LookVector.y} {cc.LookVector.z}");
            playerDebugHudUI._activeAbilities.SetText($"Slope: {cc.CurrentSlopeAngle}");
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

		if (isAnyMenuOpen) return;
		if (Time.timeScale == 0)  return;
		if (!inGame) return;
		if (SR2EWarpManager.warpTo != null) return;
		switch (SystemContext.Instance.SceneLoader.CurrentSceneGroup.name) { case "StandaloneStart": case "CompanyLogo": case "LoadScene": return; }
		if (Key.Digit0.OnKeyPressed()) SR2ECommandManager.ExecuteByString("upgrade set * 10", true);
		if (Key.Digit7.OnKeyPressed()) SR2ECommandManager.ExecuteByString("infenergy true", true);
		if (Key.Digit8.OnKeyPressed()) SR2ECommandManager.ExecuteByString("infhealth", true);
		if (Key.Digit9.OnKeyPressed()) GameContext.Instance.AutoSaveDirector.SaveAllNow();
		if (Key.P.OnKeyPressed()) SR2ECommandManager.ExecuteByString("pedia unlock * false", true);
		if (Key.K.OnKeyPressed()) SR2ECommandManager.ExecuteByString("clearinv", true);
		if (Key.L.OnKeyPressed()) SR2ECommandManager.ExecuteByString("refillinv", true);
		if (Key.N.OnKeyPressed()) SR2ECommandManager.ExecuteByString("noclip", true);
		if (Key.KeypadPlus.OnKeyPressed()) SR2ECommandManager.ExecuteByString("newbucks 1000", true);
		if (Key.KeypadMinus.OnKeyPressed()) SR2ECommandManager.ExecuteByString("newbucks -1000", true);
		if (Key.LeftBracket.OnKeyPressed()) SR2ECommandManager.ExecuteByString("fastforward -1", true);
		if (Key.RightBracket.OnKeyPressed()) SR2ECommandManager.ExecuteByString("fastforward 1", true);

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
							"\n\nADD 1000 CREDITS     + " +
							"\nREMOVE 1000 CREDITS     - " +
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
