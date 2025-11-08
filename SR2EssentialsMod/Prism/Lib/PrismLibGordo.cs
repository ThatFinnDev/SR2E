using System;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;

public static class PrismLibGordo
{
    public static void SetRequiredBait(this PrismGordo gordo, IdentifiableType baitType)
    {
        if (gordo == null) return;
        if (gordoBaitDict.ContainsKey(baitType.ReferenceId)) gordoBaitDict.Remove(baitType.ReferenceId);
        gordoBaitDict.Add(baitType.ReferenceId, gordo);
    }
    internal static Dictionary<string, PrismGordo> gordoBaitDict = new Dictionary<string, PrismGordo>();
}