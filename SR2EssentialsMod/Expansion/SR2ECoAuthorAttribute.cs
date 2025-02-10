using System;
 
namespace SR2E.Expansion;
 
[AttributeUsage(AttributeTargets.Assembly)]
public class SR2ECoAuthorAttribute : Attribute
{
     public string CoAuthor = "";

     public SR2ECoAuthorAttribute(string CoAuthor)
     {
          this.CoAuthor = CoAuthor;
     }
}
