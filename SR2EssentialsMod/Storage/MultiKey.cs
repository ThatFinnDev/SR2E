using System.Linq;
using UnityEngine.InputSystem;

namespace SR2E.Storage;

public struct MultiKey
{
    public MultiKey(List<Key> requiredKeys)
    {
        this.requiredKeys = requiredKeys;
    }
    public MultiKey(params Key[] requiredKeys)
    {
        this.requiredKeys = requiredKeys.ToList();
    }
    public List<Key> requiredKeys = new List<Key>();
   
}