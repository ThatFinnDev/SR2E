using Il2CppMonomiPark.SlimeRancher;
using SR2E.Enums;
using SR2E.Expansion;
using SR2E.Prism.Lib;
using SR2E.Saving;
using SR2E.Storage;

namespace SR2EExampleExpansion;


public class ExpansionEntryPoint : SR2EExpansionV3
{
    public override void OnInitializeMelon()
    {
        AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv"));
    }
}

