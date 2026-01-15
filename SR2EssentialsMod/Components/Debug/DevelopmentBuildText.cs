using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MelonLoader.Utils;
using SR2E.Storage;

namespace SR2E.Components;
[InjectClass]
internal class DevelopmentBuildText : MonoBehaviour
{

    private int fontSize = 11;
    void OnGUI()
    {
        string text = "Development Build";

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.alignment = TextAnchor.LowerRight;

        style.normal.textColor = Color.black;
        Vector2 offset = new Vector2(1, 1);
        Rect rect = new Rect(Screen.width - 10 - 200, Screen.height - 30, 200, 30);

        GUI.Label(new Rect(rect.x - offset.x, rect.y, rect.width, rect.height), text, style);
        GUI.Label(new Rect(rect.x + offset.x, rect.y, rect.width, rect.height), text, style);
        GUI.Label(new Rect(rect.x, rect.y - offset.y, rect.width, rect.height), text, style);
        GUI.Label(new Rect(rect.x, rect.y + offset.y, rect.width, rect.height), text, style);

        style.normal.textColor = Color.white;
        GUI.Label(rect, text, style);
    }
}
