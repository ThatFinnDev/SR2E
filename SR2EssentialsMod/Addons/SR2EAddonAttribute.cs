using System;

namespace SR2E.Addons;

[AttributeUsage(AttributeTargets.Assembly)]
public class SR2EAddonAttribute : Attribute {
    
    public SR2EAddonAttribute() {}
}