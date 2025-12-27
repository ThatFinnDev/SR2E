using System;

namespace SR2E.Saving;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class StoreInSaveAttribute : Attribute { }
