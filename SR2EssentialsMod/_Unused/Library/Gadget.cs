/*
 
 
 
 TODO:
 MIGRATE TO PRISM
 IMPROVE PREFAB MECHANIC
 
 
 
 
 
 using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.World;
using SR2E.Prism.Lib;

namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    public static partial class Gadgets
    {
        public struct CreateGadgetParams
        {
            public PurchaseCost cost;
            public bool buyInPairs;
            public bool noPickup;
            public int countLimit;
            public GadgetDefinition.Types type;
            public Sprite icon;

            public CreateGadgetParams() : this(PurchaseCost.CreateEmpty())
            {
                
            } // Default Constructor

            public CreateGadgetParams(PurchaseCost cost) : this(cost, GadgetDefinition.Types.DEFAULT)
            {
            }

            public CreateGadgetParams(PurchaseCost cost, GadgetDefinition.Types type) : this(cost, type, false)
            {
            }

            public CreateGadgetParams(PurchaseCost cost, GadgetDefinition.Types type, bool buyInPairs) : this(cost,
                type, buyInPairs, false)
            {
            }

            public CreateGadgetParams(PurchaseCost cost, GadgetDefinition.Types type, bool buyInPairs, bool noPickup) :
                this(cost, type, buyInPairs, noPickup, Int32.MaxValue)
            {
            }

            public CreateGadgetParams(PurchaseCost cost, GadgetDefinition.Types type, bool buyInPairs, bool noPickup,
                int countLimit) : this(cost, type, buyInPairs, noPickup, countLimit, null)
            {
            }

            public CreateGadgetParams(PurchaseCost cost, GadgetDefinition.Types type, bool buyInPairs, bool noPickup,
                int countLimit, Sprite icon)
            {
                this.cost = cost;
                this.buyInPairs = buyInPairs;
                this.noPickup = noPickup;
                this.countLimit = countLimit;
                this.type = type;
                this.icon = icon;
            }

        }

        public static GadgetDefinition CreateGadget(string name, GameObject prefab,
            CreateGadgetParams parameters = default)
        {
            var definition = Object.Instantiate(Resources.FindObjectsOfTypeAll<GadgetDefinition>().First());

            definition.CraftingCosts = parameters.cost;
            definition.name = name;
            definition.referenceId = $"GadgetDefinition.{name}";
            definition.prefab = prefab;
            definition.Type = parameters.type;
            definition.BuyInPairs = parameters.buyInPairs;
            definition.DestroyOnRemoval = parameters.noPickup;
            definition.CountLimit = parameters.countLimit;
            definition.icon = parameters.icon;

            PrismLibSaving.SetupForSaving(definition);

            return definition;
        }

        /// <summary>
        /// Creates a new teleporter
        /// </summary>
        /// <param name="name">Name of the gadget in the code</param>
        /// <param name="cost">The cost to craft</param>
        /// <param name="color1">Example for a pink teleporter: <c>new Color(0.283f, 0.1001f, 0.1937f, 0f)</c></param>
        /// <param name="color2">Example for a pink teleporter: <c>new Color(0.8019f, 0.4861f, 0.4577f, 0.1216f)</c></param>
        /// <param name="color3">Example for a pink teleporter: <c>new Color(0.3679f, 0.1406f, 0.2364f, 0.6863f)</c></param>
        /// <param name="color4">Example for a pink teleporter: <c>new Color(0.4157f, 0.3412f, 0.3412f, 0f)</c></param>
        /// <returns></returns>
        public static GadgetDefinition CreateTeleporter(string name, PurchaseCost cost, Color color1, Color color2,
            Color color3, Color color4)
        {
            var definition = Object.Instantiate(Get<GadgetDefinition>("TeleporterPink")); // todo: get teleporter from

            definition.name = name;
            definition.referenceId = $"GadgetDefinition.{name}";
            PrismLibSaving.SetupForSaving(definition);
            definition.prefab = Object.Instantiate(definition.prefab);

            definition.prefab.GetComponent<Gadget>().identType = definition;

            
            var mat = definition.prefab.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1)
                .GetComponent<SkinnedMeshRenderer>().material;
            mat.SetColor("_Color00", color1);
            mat.SetColor("_Color01", color2);
            mat.SetColor("_Color02", color3);
            mat.SetColor("_Color10", color4);

            return definition;
        }
    }
}*/