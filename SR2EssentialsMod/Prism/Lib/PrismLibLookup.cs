using SR2E.Prism.Data;
using SR2E.Prism.Enums;

namespace SR2E.Prism.Lib;

public static class PrismLibLookup
{
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
            { PrismNativeBaseSlime.Boom, "SlimeDefinition.Boom," },
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
            { PrismNativeBaseSlime.Prisma, "SlimeDefinition.Prisma" },
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
            { PrismNativePlort.Boom, "IdentifiableType.Boom,Plort" },
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
            { PrismNativePlort.Prisma, "IdentifiableType.PrismaPlort" },
            { PrismNativePlort.Hyper, "IdentifiableType.HyperPlort" },
            { PrismNativePlort.Gold, "IdentifiableType.GoldPlort" }
        };
    
}