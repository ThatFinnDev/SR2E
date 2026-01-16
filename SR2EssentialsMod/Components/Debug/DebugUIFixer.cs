using Il2CppMonomiPark.SlimeRancher.DebugTool;
using Il2CppTMPro;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Components;

// TODO
// DebugUI contains like nothing :/
// It gets activated by instantiating
// There are 2 variants, one for keyboard, one for gamepad
// It has a prefab and some input actions
// Maybe some helper like:
// GameDebugDirectorHelper
// SceneDebugDirectorHelper
[InjectClass]
internal class DebugUIFixer : MonoBehaviour
{
    private DebugUI debugUI;
    public DebugDirectorFixer ddf;
    private DebugDirector director => ddf.director;
    void Start()
    {
        debugUI = GetComponent<DebugUI>();
        GetComponent<Canvas>().sortingOrder = 22760;
        AddButton("testbutton", null);
    }

    public void AddButton(string text,Sprite sprite)
    {
        var instance = Instantiate(debugUI.buttonPrefab, debugUI.grid.transform);
        instance.GetObjectRecursively<TextMeshProUGUI>("Name").text = text;
        instance.GetObjectRecursively<Image>("Icon").sprite = sprite;
    }
}