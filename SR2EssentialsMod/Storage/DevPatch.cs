using System;

namespace SR2E.Storage;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class DevPatch : Attribute
{
    public DevPatch(){}
}