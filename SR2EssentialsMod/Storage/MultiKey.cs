using System.Linq;
using UnityEngine.InputSystem;

namespace SR2E.Storage;

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
                if(key.kc().wasPressedThisFrame)
                {
                    wasPressed = true;
                    foreach (Key keyTwo in requiredKeys)
                        if (key != keyTwo)
                            if (!keyTwo.kc().isPressed)
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
                if(key.kc().wasReleasedThisFrame)
                {
                    wasReleased = true;
                    foreach (Key keyTwo in requiredKeys)
                        if (key != keyTwo)
                            if (!keyTwo.kc().isPressed)
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
                if(!key.kc().isPressed)
                    return false;
            return true;
        }
    }
}