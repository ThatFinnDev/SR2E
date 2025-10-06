using System;
 
namespace SR2E.Expansion;
 
[AttributeUsage(AttributeTargets.Assembly)]
[Obsolete("This is deprecated. Use: `[assembly: AssemblyMetadata(\"co_authors\",\"<coauthor, coauthor2>\")]`",true)]
public class SR2ECoAuthorAttribute : Attribute
{
     public string CoAuthor = "";

     public SR2ECoAuthorAttribute(string CoAuthor)
     {
          this.CoAuthor = CoAuthor;
     }
}
