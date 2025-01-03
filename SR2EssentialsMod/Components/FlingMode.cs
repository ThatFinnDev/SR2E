using UnityEngine.InputSystem;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class FlingMode : MonoBehaviour
{
    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SR2EConsole.ExecuteByString("fling 100");
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SR2EConsole.ExecuteByString("fling -100");
        }
    }
}