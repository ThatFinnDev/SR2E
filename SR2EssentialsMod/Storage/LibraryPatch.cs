using System;

namespace SR2E.Storage;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class LibraryPatch : Attribute
{
    public LibraryPatch() { }
}