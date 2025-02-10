using System.Linq;
using UnityEngine.InputSystem;
<<<<<<< HEAD

namespace SR2E.Storage;

public class MultiKey
{
=======
using Key = SR2E.Enums.Key;

namespace SR2E.Storage;

/// <summary>
/// Struct acting as an array of keys check for input at the same time
/// </summary>
public struct MultiKey
{
    /// <summary>
    /// A Multi-Key constructor using a List instead of an array
    /// </summary>
    /// <param name="requiredKeys">The collection of keys to check for</param>
>>>>>>> experimental
    public MultiKey(List<Key> requiredKeys)
    {
        this.requiredKeys = requiredKeys;
    }
<<<<<<< HEAD
    public MultiKey(Key[] requiredKeys)
=======
    /// <summary>
    /// A Multi-Key constructor using a params array.
    /// </summary>
    /// <param name="requiredKeys">The collection of keys to check for</param>
    public MultiKey(params Key[] requiredKeys)
>>>>>>> experimental
    {
        this.requiredKeys = requiredKeys.ToList();
    }
    public List<Key> requiredKeys = new List<Key>();
<<<<<<< HEAD

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
=======
   
>>>>>>> experimental
}