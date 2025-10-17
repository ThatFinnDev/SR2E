using Il2CppTMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]

public class ClickableTextLink : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Canvas canvas;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        // Safely find any canvas in the hierarchy
        canvas = Find<Canvas>(transform);
        if(canvas==null) Destroy(this);
    }
    T Find<T>(Transform obj) where T : Component
    {
        var canvas = obj.gameObject.GetComponent<T>();
        if (canvas != null) return canvas;
        if(obj.GetParent()!=null) return Find<T>(obj.GetParent());
        return null;
    }
    void Click(Vector2 pos)
    {
        var cam = (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, cam);
        if (linkIndex == -1) return;
        string id = text.textInfo.linkInfo[linkIndex].GetLinkID();

        MelonLogger.Msg(id);
        if (id.StartsWith("http://")||id.StartsWith("https://")) Application.OpenURL(id);
    }
    void Update()
    {
        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Click(Mouse.current.position.ReadValue());
            return;
        }
        if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
            Click(Touchscreen.current.primaryTouch.position.ReadValue());
    }
}
