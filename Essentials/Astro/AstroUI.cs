using Starlight.Astro.Components;

namespace Starlight.Astro;

internal static class AstroUI
{
    private static bool _initialized = false;

    internal static GameObject AstroRoot;
    internal static GameObject AstroToolbar;
    
    internal static void Initialize(SystemContext systemContext)
    {
        if (_initialized) return;
        if (!EnableAstroMenu.HasFlag()) return;

        AstroRoot = new GameObject("AstroRoot");
        Object.DontDestroyOnLoad(AstroRoot);
        var canvas = AstroRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        var scaler = AstroRoot.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        AstroRoot.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        AstroToolbar = new GameObject("AstroToolbar");
        AstroToolbar.transform.SetParent(AstroRoot.transform, false);
        AstroToolbar.AddComponent<AstroToolbar>();

        var astroInfo = new GameObject("AstroInfo");
        astroInfo.transform.SetParent(AstroRoot.transform, false);
        astroInfo.AddComponent<AstroUIInfo>();

        var mouseIndicator = new GameObject("AstroMouseIndicator");
        mouseIndicator.transform.SetParent(AstroRoot.transform, false);
        mouseIndicator.AddComponent<AstroMouseIndicator>();

        AstroRoot.AddComponent<Components.AstroTransformGizmo>();
        AstroRoot.AddComponent<AstroTransformInspector>();
        
        _initialized = true;
    }

    internal static void OnGUI()
    {
        if (!_initialized) return;
    }
}