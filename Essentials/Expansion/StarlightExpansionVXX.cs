using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Starlight.Storage;

namespace Starlight.Expansion;

[EditorBrowsable(EditorBrowsableState.Never)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class StarlightExpansionVXX
{
    protected StarlightExpansionVXX() {}
    protected abstract StarlightPackageInfo info { get; }
    internal StarlightPackageInfo StarlightInternal_GetInfo => info;
    
    public Assembly Assembly => _assembly;
    private Assembly _assembly;
    public bool IsLoaded => _isLoaded;
    internal bool _isLoaded = false;
    public HarmonyLib.Harmony HarmonyInstance => _harmonyInstance;
    private HarmonyLib.Harmony _harmonyInstance;

}