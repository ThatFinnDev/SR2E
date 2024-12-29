using System;
 
namespace SR2E.Expansion;
 
[AttributeUsage(AttributeTargets.Assembly)]
public class SR2EDisplayVersion : Attribute
{
    public string Version = "";

    public SR2EDisplayVersion(string Version)
    {
        this.Version = Version;
    }
}