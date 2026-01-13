using System;
using SR2E.Enums;

namespace SR2E.Storage;


[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class CallOnAttribute : Attribute
{
    public CallEvent callEvent { get; }

    public CallOnAttribute(CallEvent callEvent)
    {
        this.callEvent = callEvent;
    }
}