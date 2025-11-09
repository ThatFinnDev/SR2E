using System;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;
/// <summary>
/// A library of helper functions for dealing with gordos
/// </summary>
public static class PrismLibGordo
{
    /// <summary>
    /// Sets the required bait for a gordo
    /// </summary>
    /// <param name="gordo">The gordo to set the bait for</param>
    /// <param name="baitType">The bait to set</param>
    public static void SetRequiredBait(this PrismGordo gordo, IdentifiableType baitType)
    {
        if (gordo == null) return;
        if (gordoBaitDict.ContainsKey(baitType.ReferenceId)) gordoBaitDict.Remove(baitType.ReferenceId);
        gordoBaitDict.Add(baitType.ReferenceId, gordo);
    }
    internal static Dictionary<string, PrismGordo> gordoBaitDict = new Dictionary<string, PrismGordo>();
}