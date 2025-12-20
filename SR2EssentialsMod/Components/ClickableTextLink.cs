using System;
using System.Linq;
using System.Reflection;
using Il2CppTMPro;
using SR2E.Enums.Sounds;
using SR2E.Storage;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E.Components;

/// <summary>
/// "ClickableTextLink" is a MonoBehaviour you can add every TextMeshProUGUI
/// It makes text, that is marked as a link, clickable
/// By default https:// and http:// is supported
/// If you want custom actions, use "action:somekey" where somekey is a string of your choice
/// Add the an action to the dictionary "actions"
/// </summary>
[InjectClass]
public class ClickableTextLink : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Canvas canvas;
    /// <summary>
    /// The Dictionary to specify all custom actions
    /// </summary>
    public Dictionary<string, System.Action> actions = new Dictionary<string, Action>();
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if(text==null) Destroy(this);
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
    bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new Il2CppSystem.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var r in results)
        {
            if (r.gameObject !=gameObject) return true;
            break;
        }
        return false;
    }
    void Click(Vector2 pos)
    {
        if (IsPointerOverUI(pos)) return;
        var cam = (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, cam);
        if (linkIndex == -1) return;
        string id = text.textInfo.linkInfo[linkIndex].GetLinkID();

        AudioEUtil.PlaySound(MenuSound.Click);
        if (id.StartsWith("http://")||id.StartsWith("https://")) Application.OpenURL(id);
        if (id.StartsWith("action:"))
        {
            string key = id.Substring(7);
            if (actions.ContainsKey(key))
            {
                try
                {
                    actions[key].Invoke();
                }
                catch (Exception e) { MelonLogger.Error(e); }
            }
        }
        /*if (id.StartsWith("callstatic:"))
        {
            string full = id.Substring(11);
            MelonLogger.Msg("Try calling");
            MelonLogger.Msg(full);
            try
            {
                full = full.Replace("()", "");
                int i = full.LastIndexOf('.');
                string typeName = full[..i];
                string methodName = full[(i + 1)..];

                Type t = null;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();;
                foreach (var asm in assemblies)
                    if ((t = asm.GetType(typeName)) != null) break;
                MelonLogger.Msg(t.FullName);
                MelonLogger.Msg(methodName);
                var info = t.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                if(info==null)
                    MelonLogger.Msg("Couldn't run click action! Method wasn't found");
                else info.Invoke(null,null);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                
            }

        }*/
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
