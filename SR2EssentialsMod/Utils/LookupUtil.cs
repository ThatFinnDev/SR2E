using Il2CppMonomiPark.SlimeRancher.Weather;
using UnityEngine.InputSystem;

namespace SR2E.Utils;

public static class LookupUtil
{

    public static bool isGadget(this IdentifiableType type) => type.TryCast<GadgetDefinition>() != null;
    /// <summary>
    /// Get an IdentifiableType either by its code name or localized name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>IdentifiableType</returns>
    public static IdentifiableType GetIdentByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        name = name.ToUpper();
        if (name == "NONE" || name == "PLAYER") return null;
        foreach (IdentifiableType type in identifiableTypes) if (type.name.ToUpper() == name) return type;
        foreach (IdentifiableType type in identifiableTypes) try { if (type.GetCompactUpperName() == name.Replace("_", "")) return type; }catch { }
        return null;
    }

    public static GadgetDefinition GetGadgetDefByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        name = name.ToUpper();
        if (name == "NONE" || name == "PLAYER") return null;
        GadgetDefinition[] ids = Resources.FindObjectsOfTypeAll<GadgetDefinition>();
        foreach (GadgetDefinition type in ids) if (type.name.ToUpper() == name) return type;
        foreach (GadgetDefinition type in ids) try { if (type.GetCompactUpperName() == name.Replace("_", "")) return type; }catch { }
        return null;
    }

    public static WeatherStateDefinition GetWeatherStateByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        name = name.ToUpper();

        foreach (WeatherStateDefinition state in weatherStateDefinitions)
            if (state.GetCompactUpperName() == name)
                return state;
        return null;
    }


    public static List<string> GetVaccableListByPartialName(string input, bool useContain)
    {
        IdentifiableType[] types = vaccableTypes;
        if (string.IsNullOrWhiteSpace(input))
        {
            List<string> cleanList = new List<string>();
            int j = 0;
            foreach (IdentifiableType type in types)
            {
                bool isGadget = type.isGadget();
                if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                if (j > MAX_AUTOCOMPLETE.Get()) break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.StartsWith("!")) continue;
                        j++;
                        cleanList.Add(localizedString.Replace(" ", ""));
                    }
                }
                catch
                {
                }
            }

            cleanList.Sort();
            return cleanList;
        }

        List<string> list = new List<string>();
        List<string> listTwo = new List<string>();
        int i = 0;
        foreach (IdentifiableType type in types)
        {
            if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;

            if (i > MAX_AUTOCOMPLETE.Get()) break;
            try
            {
                if (type.LocalizedName != null)
                {
                    string localizedString = type.LocalizedName.GetLocalizedString();
                    if (localizedString.ToLower().Replace(" ", "").StartsWith(input.ToLower()))
                    {
                        if (localizedString.StartsWith("!")) continue;
                        i++;
                        list.Add(localizedString.Replace(" ", ""));
                    }
                }
            }
            catch
            {
            }
        }

        if (useContain)
            foreach (IdentifiableType type in types)
            {
                if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;

                if (i > MAX_AUTOCOMPLETE.Get()) break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.ToLower().Replace(" ", "").Contains(input.ToLower()))
                            if (!list.Contains(localizedString.Replace(" ", "")))
                            {
                                if (localizedString.StartsWith("!")) continue;
                                i++;
                                listTwo.Add(localizedString.Replace(" ", ""));
                            }
                    }
                }
                catch
                {
                }
            }

        list.Sort();
        listTwo.Sort();
        list.AddRange(listTwo);
        return list;
    }

    public static List<string> GetIdentListByPartialName(string input, bool includeNormal, bool includeGadget,
        bool useContain, bool includeStars = false)
    {
        if (!includeGadget && !includeNormal)
            if (includeStars) return new List<string>() { "*" };
            else return new List<string>();
        if (string.IsNullOrWhiteSpace(input))
        {
            List<string> cleanList = new List<string>();
            int j = 0;
            foreach (IdentifiableType type in identifiableTypes)
            {
                bool isGadget = type.isGadget();
                if (type.ReferenceId.ToLower().Contains("Gordo")) continue;
                if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                if (!includeGadget && isGadget) continue;
                if (!includeNormal && !isGadget) continue;
                if (j > MAX_AUTOCOMPLETE.Get()) break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.StartsWith("!")) continue;
                        j++;
                        cleanList.Add(localizedString.Replace(" ", ""));
                    }
                }
                catch
                {
                }
            }

            cleanList.Add("*");
            cleanList.Sort();
            return cleanList;
        }

        List<string> list = new List<string>();
        List<string> listTwo = new List<string>();
        int i = 0;
        foreach (IdentifiableType type in identifiableTypes)
        {
            bool isGadget = type.isGadget();
            if (type.ReferenceId.ToLower().Contains("Gordo")) continue;
            if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
            if (!includeGadget && isGadget) continue;
            if (!includeNormal && !isGadget) continue;

            if (i > MAX_AUTOCOMPLETE.Get()) break;
            try
            {
                if (type.LocalizedName != null)
                {
                    string localizedString = type.LocalizedName.GetLocalizedString();
                    if (localizedString.ToLower().Replace(" ", "").StartsWith(input.ToLower()))
                    {
                        if (localizedString.StartsWith("!")) continue;
                        i++;
                        list.Add(localizedString.Replace(" ", ""));
                    }
                }
            }
            catch
            {
            }
        }

        if (useContain)
            foreach (IdentifiableType type in identifiableTypes)
            {
                bool isGadget = type.isGadget();
                if (type.ReferenceId.ToLower().Contains("Gordo")) continue;
                if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                if (!includeGadget && isGadget) continue;
                if (!includeNormal && !isGadget) continue;

                if (i > MAX_AUTOCOMPLETE.Get()) break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.ToLower().Replace(" ", "").Contains(input.ToLower()))
                            if (!list.Contains(localizedString.Replace(" ", "")))
                            {
                                if (localizedString.StartsWith("!")) continue;
                                i++;
                                listTwo.Add(localizedString.Replace(" ", ""));
                            }
                    }
                }
                catch
                {
                }
            }

        list.Sort();
        listTwo.Sort();
        list.AddRange(listTwo);
        return list;
    }

    public static List<string> GetKeyListByPartialName(string input, bool useContain)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            List<string> nullList = new List<string>();
            foreach (Key key in System.Enum.GetValues<Key>())
                if (key != Key.None)
                    if (key.ToString().ToLower().StartsWith(input.ToLower()))
                        nullList.Add(key.ToString());
            nullList.Sort();
            return nullList;
        }

        List<string> list = new List<string>();
        List<string> listTwo = new List<string>();
        foreach (Key key in System.Enum.GetValues<Key>())
            if (key != Key.None)
                if (key.ToString().ToLower().StartsWith(input.ToLower()))
                    list.Add(key.ToString());
        if (useContain)
            foreach (Key key in System.Enum.GetValues<Key>())
                if (key != Key.None)
                    if (key.ToString().ToLower().Contains(input.ToLower()))
                        if (!list.Contains(key.ToString()))
                            listTwo.Add(key.ToString());
        list.Sort();
        listTwo.Sort();
        list.AddRange(listTwo);
        return list;
    }


}