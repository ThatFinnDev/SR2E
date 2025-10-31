using System;

namespace SR2E.Expansion;

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