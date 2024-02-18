using UnityEngine.InputSystem;

namespace SR2E;

[RegisterTypeInIl2Cpp(false)]
internal class SR2EDebugDirector : MonoBehaviour
{
	internal static bool isEnabled;
	private Font _helpFont;
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
		if (Keyboard.current.digit8Key.wasPressedThisFrame) SR2EConsole.ExecuteByString("invincible", true);
		if (Keyboard.current.digit9Key.wasPressedThisFrame) GameContext.Instance.AutoSaveDirector.SaveGame();
		if (Keyboard.current.kKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("clearinv", true);
		if (Keyboard.current.lKey.wasPressedThisFrame) SR2EConsole.ExecuteByString("refillslots", true);
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
