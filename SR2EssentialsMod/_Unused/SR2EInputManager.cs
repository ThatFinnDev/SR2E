using System;
using System.Linq;
using SR2E.Storage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SR2E.Managers;

/// <summary>
/// Taken from https://github.com/Atmudia/SRLE/blob/sr2/Utils/InputManager.cs
/// </summary>
[Obsolete("OBSOLETE!: Please use InputEUtil instead!",true)] 
public static class SR2EInputManager
{
    [Obsolete("OBSOLETE!: Please use Mouse.current",true)] public static Vector2 MousePosition => Mouse.current.position.ReadValue();
    [Obsolete("OBSOLETE!: Please use Mouse.current",true)] public static Vector2 MouseScrollDelta => Mouse.current.scroll.ReadValue();

    [Obsolete("OBSOLETE!: Please use Mouse.current",true)] public static bool GetMouseButtonDown(int btn)
    {
        return btn switch
        {
            0 => Mouse.current.leftButton.wasPressedThisFrame,
            1 => Mouse.current.rightButton.wasPressedThisFrame,
            2 => Mouse.current.middleButton.wasPressedThisFrame,
            _ => false
        };
    }
    [Obsolete("OBSOLETE!: Please use Mouse.current",true)] public static bool GetMouseButtonUp(int btn)
    {
        return btn switch
        {
            0 => Mouse.current.leftButton.wasReleasedThisFrame,
            1 => Mouse.current.rightButton.wasReleasedThisFrame,
            2 => Mouse.current.middleButton.wasReleasedThisFrame,
            _ => false
        };
    }

    [Obsolete("OBSOLETE!: Please use Mouse.current",true)] public static bool GetMouseButton(int btn)
    {
        return btn switch
        {
            0 => Mouse.current.leftButton.isPressed,
            1 => Mouse.current.rightButton.isPressed,
            2 => Mouse.current.middleButton.isPressed,
            _ => false
        };
    }

    [Obsolete("OBSOLETE!: Please use LKey.OnKey instead!",true)] public static bool GetKey(Key key) => Keyboard.current[key].isPressed;
    
    [Obsolete("OBSOLETE!: Please use LKey.OnKeyDown instead!",true)] public static bool GetKeyDown(Key key) => Keyboard.current[key].wasPressedThisFrame;
    

    [Obsolete("OBSOLETE!: Please use LKey.OnKeyDown instead!",true)] public static bool OnKeyPressed(this Key key) => GetKeyDown(key);
    [Obsolete("OBSOLETE!: Please use LKey.OnKey instead!",true)] public static bool OnKey(this Key key) => GetKey(key);

    [Obsolete("OBSOLETE!: Please use LMultiKey.OnKeyDown instead!",true)] public static bool OnKeyPressed(this MultiKey multiKey)
    {
        int i = 0;
        bool wasThisFrame = false;
        foreach (var key in multiKey.requiredKeys)
        {
            if (key.OnKey())
                i++;

            if (wasThisFrame)
                continue;
            if (key.OnKeyPressed())
                wasThisFrame = true;
        }

        return i == multiKey.requiredKeys.Count && wasThisFrame;
    }
    
}