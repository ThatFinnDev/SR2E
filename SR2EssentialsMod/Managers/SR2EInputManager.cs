using System;
using System.Runtime.InteropServices;
using SR2E.Storage;

namespace SR2E.Managers;


public enum KeyState
{
    Released, JustPressed, Pressed, JustReleased,
}
public static class SR2EInputManager
{
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
    
    static KeyState[] keyStates = new KeyState[512];

    internal static void Update()
    {
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            KeyState state = keyStates[(int)key];
            bool isPressed = false;
            if (Application.isFocused) isPressed = (GetAsyncKeyState((int)key) & 0x8000) != 0;
            if (isPressed && state == KeyState.Released) state=KeyState.JustPressed;
            else if (isPressed && state == KeyState.JustPressed) state=KeyState.Pressed;
            else if (isPressed && state == KeyState.Pressed) break;
            else if (!isPressed && state == KeyState.JustPressed) state=KeyState.JustReleased;
            else if (!isPressed && state == KeyState.Pressed) state=KeyState.JustReleased;
            else if (!isPressed && state == KeyState.JustReleased) state=KeyState.Released;
            else state = KeyState.Released;
            keyStates[(int)key] = state;
        }
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

