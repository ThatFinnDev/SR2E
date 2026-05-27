using System;
using Starlight.Enums.Features;

namespace Starlight.Storage;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class FeatureFlagDependentPatch : Attribute
{
    
    public readonly FeatureFlag[] Flags = null;
    public FeatureFlagDependentPatch(params FeatureFlag[] flags)
    {
        this.Flags = flags;
    }
}