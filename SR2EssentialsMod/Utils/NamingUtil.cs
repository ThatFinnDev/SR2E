using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Utils;

public static class NamingUtil
{
    public static string GetName(this IdentifiableType type)
    {
        try
        {
            string itemName = "";
            string name = type.LocalizedName.GetLocalizedString();
            if (name.Contains(" ")) itemName = "'" + name + "'";
            else itemName = name;
            return itemName;
        }
        catch
        { return type.name; }
    }
    public static string GetCompactName(this IdentifiableType type)
    {
        try
        {
            string itemName = type.LocalizedName.GetLocalizedString().Replace(" ","").Replace("_","");
            return itemName;
        }
        catch
        { return type.name.Replace(" ","").Replace("_",""); }
    }
    public static string GetCompactUpperName(this IdentifiableType type)
    {
        if (type == null) return null;
        try
        {
            string itemName = type.LocalizedName.GetLocalizedString().Replace(" ","").Replace("_","");
            return itemName.ToUpper();
        }
        catch
        { return type.name.Replace(" ","").Replace("_","").ToUpper(); }
    }
    public static string GetCompactName(this WeatherStateDefinition definition)
    {
        try { return definition.name.Replace(" ","").Replace("_",""); } catch {  }
        return null;
    }
    public static string GetCompactUpperName(this WeatherStateDefinition definition)
    {
        try { return definition.name.Replace(" ","").Replace("_","").ToUpper(); } catch {  }
        return null;
    }
}