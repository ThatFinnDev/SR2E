using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Weather.Activity;
using Il2CppMonomiPark.SlimeRancher.Weather;
using System.Linq;

namespace SR2E;

internal class ChaosMode
{


    public static void PinkTarr()
    {
        Color32 hotPink1 = new Color32(255, 0, 183, 255);
        Color32 hotPink2 = new Color32(222, 11, 162, 255);
        Color32 hotPink3 = new Color32(186, 13, 137, 255);
        SlimeDefinition tarrDefinition = GetSlime("Tarr");
        Material tarrMaterial = tarrDefinition.AppearancesDefault[0]._structures[0].DefaultMaterials[0];
        tarrMaterial.SetSlimeMatColors(hotPink1, hotPink2, hotPink3);
        
        tarrDefinition.prefab.GetComponent<AttackPlayer>().DamagePerAttack = 1000;
        
        var localedir = SystemContext.Instance.LocalizationDirector;
        tarrDefinition.AddProduceIdent(Get<IdentifiableType>("SpringPad"));
        tarrDefinition.RefreshEatmap();
        if (localedir.GetCurrentLocaleCode() == "en")
        {
            var tarrStr = localedir.Tables["Actor"].GetEntry("l.tarr_slime");
            tarrStr.Value = "Pink Tarr";
        }
    }

    /*public static void SetupTarrRain()
    {
        var slimeRainFields = WeatherState("Slime Rain State Fields");

        var spawnerActivity = new SpawnActorActivity()
        {
            name = "Tarr Rain",
            ActorType = GetSlime("Tarr"),
            IntensityDivisor = 0.3f,
            _intensity = 0.2f,
            SecondsBetweenSpawns = new Range()
            {
                Max = 5,
                Min = 3,
            },
            SpawnStrategy = slimeRainFields.Activities[0].Activity.Cast<SpawnActorActivity>().SpawnStrategy,
        };
        var activity = new WeatherStateDefinition.ActivityIntensityMapping()
        {
            Activity = spawnerActivity,
            Intensity = 1
        };
        
        slimeRainFields.Activities.Add(activity);

        var slimeRainValley = WeatherState("Slime Rain State Valley");
        var slimeRainStrand = WeatherState("Slime Rain State Strand");
        var slimeRainBluffs = WeatherState("Slime Rain State Bluffs");
        slimeRainValley.Activities.Add(activity);
        slimeRainStrand.Activities.Add(activity);
        slimeRainBluffs.Activities.Add(activity);


    }*/

    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName.StartsWith("environment"))
            try
            {
                GameObject net = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault((GameObject x) => x.name == "Fallthrough Net");
                if (net != null)
                {
                    Transform oceanBlade = net.getObjRec<Transform>("oceanBlade");
                    if (oceanBlade != null)
                        for (int i = 0; i < oceanBlade.childCount; i++)
                        {
                            MeshRenderer meshRenderer = oceanBlade.GetChild(i).GetComponent<MeshRenderer>();
                            if (meshRenderer != null)
                                meshRenderer.material = null;
                        }
                }
            }catch { }
        switch (sceneName)
        {
            case "GameCore":

                PinkTarr();

                GetSlime("Pink").SwitchSlimeAppearances(GetSlime("Gold"));
                GetSlime("Pink").prefab.AddComponent<SlimeFlee>();
                
                
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
                {
                    try
                    {
                        string[] split = slimes[i].Split("|");
                        string name = split[0];
                        float fullSaturation = float.Parse(split[1]);
                        float value = float.Parse(split[2]);
                        SlimeDefinition definition = GetSlime(name);
                        if(definition!=null)
                            definition.MakeSellable(value,fullSaturation);
                        IdentifiableType plort = GetPlort(name+"Plort");
                        if (plort != null)
                            plort.MakeNOTSellable();
                    }
                    catch { }
                }
                
                int largoIndex = 0;
                foreach (var slime in LibraryUtils.slimes.GetAllMembersArray())
                {
                    SlimeDefinition slimeReal = slime.Cast<SlimeDefinition>();
                    if (slimeReal.IsLargo)
                    {
                        if (largoIndex == 0)
                        {
                            slime.MakeSellable(28, 27, false);
                            MelonLogger.Msg(slime.ValidatableName);
                        }
                        else
                            slime.MakeSellable(20, 15, true);

                        largoIndex++;
                    }
                }
                
                break;
            case "MainMenuEnvironment":
                GameObject playerModel = GameObject.Find("BeatrixMainMenu");
                playerModel.transform.localScale = new Vector3(-13.3182f, 2.0545f, -0.9782f);
                playerModel.transform.localPosition = new Vector3(-394.3078f, 26.876f, -46.412f);
                break;
        }
    }
}