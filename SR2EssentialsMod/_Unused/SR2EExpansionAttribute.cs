using System;

namespace SR2E.Expansion;

[Obsolete("OBSOLETE!: This is deprecated. Use: `[assembly: AssemblyMetadata(SR2EExpansionAttributes.IsExpansion,true)] [assembly: AssemblyMetadata(SR2EExpansionAttributes.UsePrism,true)]`",false)]
[AttributeUsage(AttributeTargets.Assembly)]
public class SR2EExpansionAttribute : Attribute
{
    public bool usePrism = false;

    public SR2EExpansionAttribute()
    {
        
    }
    public SR2EExpansionAttribute(bool usePrism)
    {
        this.usePrism = usePrism;
    }
}