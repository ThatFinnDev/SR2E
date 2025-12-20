using SR2E.Storage;

namespace SR2E.Components;

[InjectClass]
internal class KeyCodeInputAcquirer : MonoBehaviour
{
    internal static KeyCodeInputAcquirer Instance;
    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
    private HashSet<KeyCode> downThisFrame = new HashSet<KeyCode>();
    private HashSet<KeyCode> upThisFrame = new HashSet<KeyCode>();

    private void Start()
    {
        if(Instance!=null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void OnGUI()
    {
        Event e = Event.current;

        if (e.isKey && e.keyCode != KeyCode.None)
        {
            if (e.type == EventType.KeyDown)
            {
                if (!pressedKeys.Contains(e.keyCode))
                {
                    pressedKeys.Add(e.keyCode);
                    downThisFrame.Add(e.keyCode);
                }
            }
            else if (e.type == EventType.KeyUp)
            {
                pressedKeys.Remove(e.keyCode);
                upThisFrame.Add(e.keyCode);
            }
        }
    }

    void LateUpdate()
    {
        downThisFrame.Clear();
        upThisFrame.Clear();
    }

    internal bool OnKey(KeyCode key) => pressedKeys.Contains(key);

    internal bool OnKeyDown(KeyCode key) => downThisFrame.Contains(key);

    internal bool OnKeyUp(KeyCode key) => upThisFrame.Contains(key);
}