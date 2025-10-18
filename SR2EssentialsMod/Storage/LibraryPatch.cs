using System;

namespace SR2E.Storage;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class LibraryPatch : Attribute
{
    public LibraryPatch(){}
}