using System.Linq;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E;

internal class ChaosMode
{
    
    [HarmonyPatch(typeof(MarketUI), "Start")]
    public static class PatchMarketUIStart
    {
        public static void Prefix(MarketUI __instance)
        {
            if (SR2EEntryPoint.chaosMode)
            {
                __instance.plorts = newSlimesAsPlortEntries.ToArray();
            }
        }
    }

    [HarmonyPatch(typeof(EconomyDirector), "InitModel")]
    public static class PatchEconomyDirectorInitModel
    {
        public static void Prefix(EconomyDirector __instance)
        {
            if (SR2EEntryPoint.chaosMode)
            {
                __instance.BaseValueMap = newValueMaps.ToArray();
            }
        }
    }
    public static List<MarketUI.PlortEntry> newSlimesAsPlortEntries = new List<MarketUI.PlortEntry>();
    public static List<EconomyDirector.ValueMap> newValueMaps = new List<EconomyDirector.ValueMap>();

    static void makeSlimesSellable(string nameWithInfo)
    {
        try
        {
            string[] split = nameWithInfo.Split("|");
            string name = split[0];
            float fullSaturation = float.Parse(split[1]);
            float value = float.Parse(split[2]);
            SlimeDefinition definition = SR2EUtils.Get<SlimeDefinition>(name);
            if(definition==null)
                return;
            newSlimesAsPlortEntries.Add(new MarketUI.PlortEntry { identType = definition });
            newValueMaps.Add(new EconomyDirector.ValueMap { Accept = definition.prefab.GetComponent<Identifiable>(), 
                FullSaturation = fullSaturation, Value = value });
        }
        catch { }
    }
    static void switchSlimeAppearances(string slimeOne, string slimeTwo)
    {
        SlimeDefinition slimeOneDef = SR2EUtils.Get<SlimeDefinition>(slimeOne);
        SlimeDefinition slimeTwoDef = SR2EUtils.Get<SlimeDefinition>(slimeTwo);

        var appearanceOne = slimeOneDef.AppearancesDefault; slimeOneDef.AppearancesDefault = slimeTwoDef.AppearancesDefault; slimeTwoDef.AppearancesDefault = appearanceOne;

        var icon = slimeOneDef.icon; slimeOneDef.icon = slimeTwoDef.icon; slimeTwoDef.icon = icon;

        var debugIcon = slimeOneDef.debugIcon; slimeOneDef.debugIcon = slimeTwoDef.debugIcon; slimeTwoDef.debugIcon = debugIcon;

    }

    public static void PinkTarr()
    {
        Color32 hotPink1 = new Color32(255, 0, 183, 255);
        Color32 hotPink2 = new Color32(222, 11, 162, 255);
        Color32 hotPink3 = new Color32(186, 13, 137, 255);
        SlimeDefinition tarrDefinition = SR2EUtils.Get<SlimeDefinition>("Tarr");
        Material tarrMaterial = tarrDefinition.AppearancesDefault[0]._structures[0].DefaultMaterials[0];
        tarrMaterial.SetColor("_TopColor", hotPink1);
        tarrMaterial.SetColor("_MiddleColor", hotPink2);
        tarrMaterial.SetColor("_BottomColor", hotPink3);
        tarrDefinition.prefab.GetComponent<AttackPlayer>().DamagePerAttack = 1000;
        var localedir = SystemContext.Instance.LocalizationDirector;
        if (localedir.GetCurrentLocaleCode() == "en")
        {
            var tarrStr = localedir.Tables["Actor"].GetEntry("l.tarr_slime");
            tarrStr.Value = "Pink Tarr";
        }
    }
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "GameCore":

                PinkTarr();

                // switchSlimeAppearances("Gold", "Pink"); Breaks pink largos

                List<string> slimes = new List<string> { 
                    "Pink|40|7", 
                    "Cotton|30|15", 
                    "Phosphor|30|15", 
                    "Tabby|30|15", 
                    "Angler|20|18",
                    "Rock|30|18", 
                    "Honey|20|18", 
                    "Boom|20|20",
                    "Puddle|20|22", 
                    "Fire|20|22",
                    "Batty|20|18", 
                    "Crystal|20|20",
                    "Hunter|20|20",
                    "Flutter|20|20", 
                    "Ringtail|15|22", 
                    "Saber|15|24",
                    "Yolky|15|28",
                    "Tangle|15|27", 
                    "Dervish|15|27", 
                    "Gold|50000|200",
                    "Lucky|15|27",
                    "Tarr|15|27",
                };

                for (int i = 0; i < slimes.Count; i++)
                    makeSlimesSellable(slimes[i]);
                break;
            case "MainMenuEnvironment":
                GameObject playerModel = GameObject.Find("BeatrixMainMenu");
                playerModel.transform.localScale = new Vector3(-13.3182f, 2.0545f, -0.9782f);
                playerModel.transform.localPosition = new Vector3(-394.3078f, 26.876f, -46.412f);
                break;
        }
    }
}