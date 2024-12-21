using System;
using System.Runtime.InteropServices;

namespace SR2E;


public enum KeyState
{
    Released, JustPressed, JustReleased, Pressed
}
public static class SR2EInputManager
{
    
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    private static KeyState[] keyStates = new KeyState[512];

    internal static void Update()
    {
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            KeyState state = keyStates[(int)key];
            bool isPressed = (GetAsyncKeyState((int)key) & 0x8000) != 0;
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
    
}

