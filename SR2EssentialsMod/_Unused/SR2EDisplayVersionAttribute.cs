using System;
 
namespace SR2E.Expansion;
 
[AttributeUsage(AttributeTargets.Assembly)]
[Obsolete("OBSOLETE!: This is deprecated. Use: `[assembly: AssemblyMetadata(\"display_version\",\"2.0.0-beta.23\")]`",true)]
public class SR2EDisplayVersion : Attribute
{
    public string Version = "";

    public SR2EDisplayVersion(string Version)
    {
        this.Version = Version;
    }
}