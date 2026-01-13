using Il2CppMonomiPark.SlimeRancher.DebugTool;
using SR2E.Storage;

namespace SR2E.Components;

// TODO
//DebugUI contains like nothing :/
//It gets activated by instantiating
// There are 2 variants, one for keyboard, one for gamepad
//It has a prefab and some input actions
// Maybe some helper like:
//GameDebugDirectorHelper
//SceneDebugDirectorHelper
[InjectClass]
internal class DebugUIFixer : MonoBehaviour
{
    private DebugUI debugUI;
    void Start()
    {
        debugUI = GetComponent<DebugUI>();
    }
}