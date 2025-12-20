using System;

namespace SR2E.Storage;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class InjectClass : Attribute
{
    internal InjectClass(){}
}