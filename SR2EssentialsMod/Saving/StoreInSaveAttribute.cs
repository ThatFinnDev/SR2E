using System;

namespace SR2E.Saving;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class StoreInSaveAttribute : Attribute { }
