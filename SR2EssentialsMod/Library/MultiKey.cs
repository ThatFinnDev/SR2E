using System.Linq;
using UnityEngine.InputSystem;

namespace SR2E.Library;

public class MultiKey
{
    public MultiKey(List<Key> requiredKeys)
    {
        this.requiredKeys = requiredKeys;
    }
    public MultiKey(Key[] requiredKeys)
    {
        this.requiredKeys = requiredKeys.ToList();
    }
    public List<Key> requiredKeys = new List<Key>();

    public bool wasPressedThisFrame
    {
        get
        {
            bool wasPressed = false;
            foreach (Key key in requiredKeys)
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    wasPressed = true;
                    foreach (Key keyTwo in requiredKeys)
                        if (key != keyTwo)
                            if (!Keyboard.current[key].isPressed)
                                wasPressed = false;
                }
            return wasPressed;
        }
    }

    public bool wasReleasedThisFrame
    {
        get
        {
            bool wasReleased = false;
            foreach (Key key in requiredKeys)
                if (Keyboard.current[key].wasReleasedThisFrame)
                {
                    wasReleased = true;
                    foreach (Key keyTwo in requiredKeys)
                        if (key != keyTwo)
                            if (!Keyboard.current[key].isPressed)
                                wasReleased = false;
                }
            return wasReleased;
        }
    }
    
    public bool isPressed
    {
        get
        {
            foreach (Key key in requiredKeys)
                if (!Keyboard.current[key].isPressed)
                    return false;
            return true;
        }
    }
}