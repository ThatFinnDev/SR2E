using System.Linq;
using SR2E.Storage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SR2E.Managers;

/// <summary>
/// Taken from https://github.com/Atmudia/SRLE/blob/sr2/Utils/InputManager.cs
/// </summary>
public static class SR2EInputManager
{
    public static Vector2 MousePosition => Mouse.current.position.ReadValue();
    public static Vector2 MouseScrollDelta => Mouse.current.scroll.ReadValue();

    public static bool GetMouseButtonDown(int btn)
    {
        return btn switch
        {
            0 => Mouse.current.leftButton.wasPressedThisFrame,
            1 => Mouse.current.rightButton.wasPressedThisFrame,
            2 => Mouse.current.middleButton.wasPressedThisFrame,
            _ => false
        };
    }
    public static bool GetMouseButtonUp(int btn)
    {
        return btn switch
        {
            0 => Mouse.current.leftButton.wasReleasedThisFrame,
            1 => Mouse.current.rightButton.wasReleasedThisFrame,
            2 => Mouse.current.middleButton.wasReleasedThisFrame,
            _ => false
        };
    }

    public static bool GetMouseButton(int btn)
    {
        return btn switch
        {
            0 => Mouse.current.leftButton.isPressed,
            1 => Mouse.current.rightButton.isPressed,
            2 => Mouse.current.middleButton.isPressed,
            _ => false
        };
    }

    public static bool GetKey(Key code)
    { 
        return Keyboard.current[code].isPressed;
    }
    public static bool GetKeyDown(Key code)
    {
        return Keyboard.current[code].wasPressedThisFrame;
    }

    public static bool OnKeyPressed(this Key code) => GetKeyDown(code);
    public static bool OnKey(this Key code) => GetKey(code);

    public static bool OnKeyPressed(this MultiKey code)
    {
        int i = 0;
        foreach (var key in code.requiredKeys)
        {
            if (key.OnKeyPressed())
                i++;
        }
        return i == code.requiredKeys.Count;
    }
    
}