using System;
using System.Linq;
using UnityEngine.InputSystem;

namespace SR2E.Storage;

/// <summary>
/// Struct acting as an array of keys check for input at the same time
/// </summary>
[Obsolete("OBSOLETE!: Use LMultiKey")]
public struct MultiKey
{
    /// <summary>
    /// A Multi-Key constructor using a List instead of an array
    /// </summary>
    /// <param name="requiredKeys">The collection of keys to check for</param>
    public MultiKey(List<Key> requiredKeys)
    {
        this.requiredKeys = requiredKeys;
    }
    /// <summary>
    /// A Multi-Key constructor using a params array.
    /// </summary>
    /// <param name="requiredKeys">The collection of keys to check for</param>
    public MultiKey(params Key[] requiredKeys)
    {
        this.requiredKeys = requiredKeys.ToList();
    }
    public List<Key> requiredKeys = new List<Key>();
   
}