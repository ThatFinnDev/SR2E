using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;

public static class PrismLibLookup
{
    
    
    public static IdentifiableTypeGroup identifiableGroup => LookupEUtil.allIdentifiableTypeGroups["IdentifiableTypesGroup"]; 
    public static IdentifiableTypeGroup vaccableGroup => LookupEUtil.allIdentifiableTypeGroups["VaccableNonLiquids"]; 
    public static IdentifiableTypeGroup toyGroup => LookupEUtil.allIdentifiableTypeGroups["ToyGroup"]; 
    public static IdentifiableTypeGroup gadgetGroup => LookupEUtil.allIdentifiableTypeGroups["GadgetGroup"]; 
    public static IdentifiableTypeGroup slimeGroup => LookupEUtil.allIdentifiableTypeGroups["SlimesGroup"]; 
    public static IdentifiableTypeGroup largoGroup => LookupEUtil.allIdentifiableTypeGroups["LargoGroup"]; 
    public static IdentifiableTypeGroup plortGroup => LookupEUtil.allIdentifiableTypeGroups["PlortGroup"]; 
    public static IdentifiableTypeGroup foodGroup => LookupEUtil.allIdentifiableTypeGroups["FoodGroup"]; 
    public static IdentifiableTypeGroup meatFoodGroup => LookupEUtil.allIdentifiableTypeGroups["MeatGroup"]; 
    public static IdentifiableTypeGroup veggieFoodGroup => LookupEUtil.allIdentifiableTypeGroups["VeggieGroup"]; 
    public static IdentifiableTypeGroup fruitFoodGroup => LookupEUtil.allIdentifiableTypeGroups["FruitGroup"]; 
    public static IdentifiableTypeGroup nectarFoodGroup => LookupEUtil.allIdentifiableTypeGroups["NectarFoodGroup"]; 
    public static IdentifiableTypeGroup chickFoodGroup => LookupEUtil.allIdentifiableTypeGroups["ChickGroup"]; 
    public static IdentifiableTypeGroup craftGroup => LookupEUtil.allIdentifiableTypeGroups["CraftGroup"]; 
    
    
    
    public static void Prism_AddToGroup(this IdentifiableType type, string groupName)
    {
        var group = LookupEUtil.allIdentifiableTypeGroups[groupName];
        if (group.IsMember(type)) return;
        group._memberTypes.Add(type);
        if (group.GetRuntimeObject().IsMember(type)) return;
        group.GetRuntimeObject()._memberTypes.Add(type);
    }
    public static void Prism_AddToGroup(this IdentifiableTypeGroup group, string groupName)
    {
        var group2 = LookupEUtil.allIdentifiableTypeGroups[groupName];
        if (group2._memberGroups.Contains(group)) return;
        group2._memberGroups.Add(group);
        if (group2.GetRuntimeObject()._memberGroups.Contains(group)) return;
        group2.GetRuntimeObject()._memberGroups.Add(group);
    }
    public static void AddToGroup(this PrismIdentifiableTypeGroup group, string groupName)
    {
        var group2 = LookupEUtil.allIdentifiableTypeGroups[groupName];
        if (group2._memberGroups.Contains(group._group)) return;
        group2._memberGroups.Add(group._group);
        if (group2.GetRuntimeObject()._memberGroups.Contains(group._group)) return;
        group2.GetRuntimeObject()._memberGroups.Add(group._group);
    }
    public static bool Prism_IsInImmediateGroup(this IdentifiableType type, string groupName)
    {
        var group = LookupEUtil.allIdentifiableTypeGroups[groupName];
        if (group == null) return false;
        return group._memberTypes.Contains(type);
    }
    public static bool IsInImmediateGroup(this PrismSlime prismSlime, string groupName)
    {
        var group = LookupEUtil.allIdentifiableTypeGroups[groupName];
        if (group == null) return false;
        return group._memberTypes.Contains(prismSlime._slimeDefinition);
    }
    public static bool IsInImmediateGroup(this PrismPlort prismPlort, string groupName)
    {
        var group = LookupEUtil.allIdentifiableTypeGroups[groupName];
        if (group == null) return false;
        return group._memberTypes.Contains(prismPlort._identifiableType);
    }
    public static bool DoesLargoComboExist(this PrismBaseSlime slime1, PrismBaseSlime slime2)
    {
        try
        {
            if (gameContext.SlimeDefinitions.GetLargoByBaseSlimes(slime1, slime2) != null)
                return true;
        } catch { }
        try
        {
            if (gameContext.SlimeDefinitions.GetLargoByBaseSlimes(slime2, slime1) != null)
                return true;
        } catch { }
        return false;
    }
    public static string GetReferenceID(this PrismNativeBaseSlime nativeBaseSlime)
    {
        if (refIDTranslationPrismNativeBaseSlime.ContainsKey(nativeBaseSlime))
            return refIDTranslationPrismNativeBaseSlime[nativeBaseSlime];
        return null;
    }
    public static string GetReferenceID(this PrismNativePlort nativePlort)
    {
        
        if (refIDTranslationPrismNativePlort.ContainsKey(nativePlort))
            return refIDTranslationPrismNativePlort[nativePlort];
        return null;
    }
    internal static Dictionary<PrismNativeBaseSlime, string> refIDTranslationPrismNativeBaseSlime =
        new Dictionary<PrismNativeBaseSlime, string>()
        {
            { PrismNativeBaseSlime.Pink, "SlimeDefinition.Pink" },
            { PrismNativeBaseSlime.Cotton, "SlimeDefinition.Cotton" },
            { PrismNativeBaseSlime.Phosphor, "SlimeDefinition.Phosphor" },
            { PrismNativeBaseSlime.Tabby, "SlimeDefinition.Tabby" },
            { PrismNativeBaseSlime.Angler, "SlimeDefinition.Angler" },
            { PrismNativeBaseSlime.Rock, "SlimeDefinition.Rock" },
            { PrismNativeBaseSlime.Honey, "SlimeDefinition.Honey" },
            { PrismNativeBaseSlime.Boom, "SlimeDefinition.Boom" },
            { PrismNativeBaseSlime.Puddle, "SlimeDefinition.Puddle" },
            { PrismNativeBaseSlime.Fire, "SlimeDefinition.Fire" },
            { PrismNativeBaseSlime.Batty, "SlimeDefinition.Batty" },
            { PrismNativeBaseSlime.Crystal, "SlimeDefinition.Crystal" },
            { PrismNativeBaseSlime.Hunter, "SlimeDefinition.Hunter" },
            { PrismNativeBaseSlime.Flutter, "SlimeDefinition.Flutter" },
            { PrismNativeBaseSlime.Ringtail, "SlimeDefinition.Ringtail" },
            { PrismNativeBaseSlime.Saber, "SlimeDefinition.Saber" },
            { PrismNativeBaseSlime.Yolky, "SlimeDefinition.Yolky" },
            { PrismNativeBaseSlime.Tangle, "SlimeDefinition.Tangle" },
            { PrismNativeBaseSlime.Dervish, "SlimeDefinition.Dervish" },
            { PrismNativeBaseSlime.Twin, "SlimeDefinition.Twin" },
            { PrismNativeBaseSlime.Sloomber, "SlimeDefinition.Sloomber" },
            { PrismNativeBaseSlime.Shadow, "SlimeDefinition.Shadow" },
            { PrismNativeBaseSlime.Hyper, "SlimeDefinition.Hyper" },
            { PrismNativeBaseSlime.Gold, "SlimeDefinition.Gold" },
            { PrismNativeBaseSlime.Lucky, "SlimeDefinition.Lucky" },
            { PrismNativeBaseSlime.Tarr, "SlimeDefinition.Tarr" },
        };
    internal static Dictionary<PrismNativePlort, string> refIDTranslationPrismNativePlort =
        new Dictionary<PrismNativePlort, string>()
        {
            { PrismNativePlort.Pink, "IdentifiableType.PinkPlort" },
            { PrismNativePlort.Cotton, "IdentifiableType.CottonPlort" },
            { PrismNativePlort.Phosphor, "IdentifiableType.PhosphorPlort" },
            { PrismNativePlort.Tabby, "IdentifiableType.TabbyPlort" },
            { PrismNativePlort.Angler, "IdentifiableType.AnglerPlort" },
            { PrismNativePlort.Rock, "IdentifiableType.RockPlort" },
            { PrismNativePlort.Honey, "IdentifiableType.HoneyPlort" },
            { PrismNativePlort.Boom, "IdentifiableType.BoomPlort" },
            { PrismNativePlort.Puddle, "IdentifiableType.PuddlePlort" },
            { PrismNativePlort.Fire, "IdentifiableType.FirePlort" },
            { PrismNativePlort.Batty, "IdentifiableType.BattyPlort" },
            { PrismNativePlort.Crystal, "IdentifiableType.CrystalPlort" },
            { PrismNativePlort.Hunter, "IdentifiableType.HunterPlort" },
            { PrismNativePlort.Flutter, "IdentifiableType.FlutterPlort" },
            { PrismNativePlort.Ringtail, "IdentifiableType.RingtailPlort" },
            { PrismNativePlort.Saber, "IdentifiableType.SaberPlort" },
            { PrismNativePlort.Yolky, "IdentifiableType.YolkyPlort" },
            { PrismNativePlort.Tangle, "IdentifiableType.TanglePlort" },
            { PrismNativePlort.Dervish, "IdentifiableType.DervishPlort" },
            { PrismNativePlort.Twin, "IdentifiableType.TwinPlort" },
            { PrismNativePlort.Sloomber, "IdentifiableType.SloomberPlort" },
            { PrismNativePlort.Shadow, "IdentifiableType.ShadowPlort" },
            { PrismNativePlort.Prisma, "IdentifiableType.StablePlort" },
            { PrismNativePlort.Hyper, "IdentifiableType.HyperPlort" },
            { PrismNativePlort.Gold, "IdentifiableType.GoldPlort" },
            { PrismNativePlort.Unstable, "IdentifiableType.UnstablePlort" }
        };
    
}