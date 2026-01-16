using System;

namespace SR2E.Enums;

internal class DebugUIEntry
{
    public string text = "<Missing Text>";
    public Sprite icon = null;
    public bool closesMenu = true;
    public Action action;
}