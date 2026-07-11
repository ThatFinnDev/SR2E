using Starlight.Astro.Components;

namespace Starlight.Astro;

internal static class AstroMode
{
    internal enum Mode { Gameplay, Transform }
    
    internal static Mode Current = Mode.Gameplay;

    internal static void SetMode(Mode mode)
    {
        if (Current == mode) return;
        var previous = Current;
        Current = mode;
        
        // cleanup previous mode
        switch (previous)
        {
            case Mode.Transform:
                AstroUIInfo.ClearToolInfo();
                break;
        }

        if (mode == Mode.Transform)
        {
            if (AstroTransformInspector.Instance && AstroTransformInspector.Instance.WindowRoot)
                AstroTransformInspector.Instance.WindowRoot.SetActive(true);
        }
        
        Log($"[Astro/Mode] {previous} → {mode}");
    }
}
