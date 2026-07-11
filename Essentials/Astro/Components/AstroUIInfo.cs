using Il2CppTMPro;
using Starlight.Storage;

namespace Starlight.Astro.Components;

[InjectIntoIL]
internal class AstroUIInfo : MonoBehaviour
{
    private static AstroUIInfo _instance;
    private TextMeshProUGUI _text;
    private string _toolInfo = string.Empty;

    internal static AstroUIInfo Instance => _instance;

    void Start()
    {
        _instance = this;

        var rect = gameObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(8, -22);
        rect.sizeDelta = new Vector2(400, 200);

        _text = gameObject.AddComponent<TextMeshProUGUI>();
        _text.fontSize = 11f;
        _text.color = AstroUITheme.TextColor;
        _text.alignment = TextAlignmentOptions.TopLeft;
        _text.raycastTarget = false;
        _text.richText = true;
        _text.overflowMode = TextOverflowModes.Overflow;
        _text.enableWordWrapping = false;

        var fontTheme = new UI.FontTheme();
        if (fontTheme.DefaultFont)
            _text.font = fontTheme.DefaultFont;
    }

    void Update()
    {
        if (!_text) return;

        var cam = MiscEUtil.GetActiveCamera();
        var camLine = cam ? $"Camera Pos: {F(cam.transform.position)}" : "Camera Pos: ? ? ?";

        var playerLine = "";
        try
        {
            if (sceneContext && sceneContext.Player)
                playerLine = $"Player Pos: {F(sceneContext.Player.transform.position)}";
            else
                playerLine = "Player Pos: ? ? ?";
        }
        catch
        {
            playerLine = "Player Pos: ? ? ?";
        }

        _text.text = string.IsNullOrEmpty(_toolInfo) ? $"{camLine}\n{playerLine}" : $"{camLine}\n{playerLine}\n\n{_toolInfo}";
    }

    internal static void SetToolInfo(string info)
    {
        if (_instance) 
            _instance._toolInfo = info ?? string.Empty;
    }

    internal static void AppendToolInfo(string line)
    {
        if (_instance)
        {
            if (string.IsNullOrEmpty(_instance._toolInfo))
                _instance._toolInfo = line;
            else
                _instance._toolInfo += "\n" + line;
        }
    }

    internal static void ClearToolInfo()
    {
        if (_instance) _instance._toolInfo = string.Empty;
    }

    private static string F(Vector3 v) => $"{v.x:F2} {v.y:F2} {v.z:F2}";
}
