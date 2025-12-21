using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;

namespace SR2E.Components;

[InjectClass]
internal class FlingMode : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SR2ECommandManager.ExecuteByString("fling 100");
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SR2ECommandManager.ExecuteByString("fling -100");
        }
    }
}