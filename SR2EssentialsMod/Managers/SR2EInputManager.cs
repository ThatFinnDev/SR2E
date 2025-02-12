using System;
using System.Runtime.InteropServices;
using SR2E.Enums;
using SR2E.Storage;

namespace SR2E.Managers;


public static class SR2EInputManager
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);
    
    static KeyState[] keyStates = new KeyState[512];

    internal static void Update()
    {
        foreach (Key key in Enum.GetValues(typeof(Key)))
            keyStates[(int)key] = (KeyState)GetAsyncKeyState((int)key);
        
    }
    public static bool OnKeyPressed(this Key key) => keyStates[(int)key]==KeyState.JustPressed;
    public static bool OnKeyUnpressed(this Key key) => keyStates[(int)key]==KeyState.JustReleased;
    public static bool OnKey(this Key key) => keyStates[(int)key]==KeyState.Pressed;
    
    /// <returns>If the key was pressed this frame</returns>
    public static bool OnKeyPressed(this MultiKey multiKey)
    {
        foreach (Key key in multiKey.requiredKeys)
        {
            if (key.OnKeyPressed())
            {
                bool allKeysPressed = true;
                foreach (Key requiredKey in multiKey.requiredKeys)
                    if (!(requiredKey.OnKey() || requiredKey.OnKeyPressed()))
                    {
                        allKeysPressed = false;
                        break;
                    }
                if (allKeysPressed) return true;
            }
        }
        return false;
    }
    
    public static bool OnKeyUnpressed(this MultiKey multiKey)
    {
        foreach (Key key in multiKey.requiredKeys)
        {
            if (key.OnKeyUnpressed())
            {
                bool allKeysUnpressed = true;
                foreach (Key requiredKey in multiKey.requiredKeys)
                    if (!(requiredKey.OnKey() || requiredKey.OnKeyUnpressed()))
                    {
                        allKeysUnpressed = false;
                        break;
                    }
                if (allKeysUnpressed) return true;
            }
        }
        return false;
    }
    
    public static bool OnKey(this MultiKey multiKey)
    {
        foreach (Key key in multiKey.requiredKeys) if(!key.OnKey()) return false;
        return true;
    }
}

