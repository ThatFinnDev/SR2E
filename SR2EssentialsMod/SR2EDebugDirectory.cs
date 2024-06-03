using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI.Debug;
using Il2CppTMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E;

[RegisterTypeInIl2Cpp(false)]
internal class SR2EDebugDirector : MonoBehaviour
{
	internal static bool isEnabled;
	private Font _helpFont;
	internal class DebugStatsManager
    {
        internal static bool playerDebugUIEnabled = false;
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
            playerDebugHudUI.GetComponent<VerticalLayoutGroup>().spacing = -50;
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

		if (SR2EConsole.isOpen) return;
		if (SR2EModMenu.isOpen) return;
		if (Time.timeScale == 0)  return;
		
		if (Keyboard.current.digit0Key.wasPressedThisFrame) SR2EConsole.ExecuteByString("giveupgrades *", true);
		if (Keyboard.current.digit7Key.wasPressedThisFrame) SR2EConsole.ExecuteByString("infenergy true", true);
		if (Keyboard.current.digit8Key.wasPressedThisFrame) SR2EConsole.ExecuteByString("infhealth", true);
		if (Keyboard.current.digit9Key.wasPressedThisFrame) GameContext.Instance.AutoSaveDirector.SaveGame();
		if (Keyboard.current.kKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("clearinv", true);
		if (Keyboard.current.lKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("refillinv", true);
		if (Keyboard.current.nKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("noclip", true);
		if (Keyboard.current.numpadPlusKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("newbucks 1000", true);
		if (Keyboard.current.numpadMinusKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("newbucks -1000", true);
		if (Keyboard.current.leftBracketKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("fastforward -1", true);
		if (Keyboard.current.rightBracketKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("fastforward 1", true);

	}

	private void OnGUI()
	{
		if (isEnabled)
		{
			GUI.skin.label.font = _helpFont;
			GUI.skin.label.alignment = TextAnchor.UpperRight;
			string text = "<b>DEBUG MODE INFO" +
			              " \n\nGIVE ALL PERSONAL UPGRADES     0 " +
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
