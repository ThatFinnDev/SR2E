using Il2CppMonomiPark.SlimeRancher.Economy;

namespace SR2E.Utils;

public static class CurrencyEUtil
{
    public static ICurrency toICurrency(this CurrencyDefinition currencyDefinition) => currencyDefinition.TryCast<ICurrency>();
    public static CurrencyDefinition toCurrency(this ICurrency iCurrency) => iCurrency.TryCast<CurrencyDefinition>();
    public static bool SetCurrency(string referenceID, int amount)
    {
        if (string.IsNullOrWhiteSpace(referenceID)) return false;
        if (!inGame) return false;
        var id = referenceID;
        if (!id.StartsWith("CurrencyDefinition.")) id = "CurrencyDefinition." + id;

        var def = gameContext.LookupDirector.CurrencyList.FindCurrencyByReferenceId(id);
        sceneContext.PlayerState._model.SetCurrency(def.toICurrency(), amount);
        return true;
    }

    public static bool SetCurrency(string referenceID, int amount, int amountEverCollected)
    {
        if (string.IsNullOrWhiteSpace(referenceID)) return false;
        if (!inGame) return false;
        var id = referenceID;
        if (!id.StartsWith("CurrencyDefinition.")) id = "CurrencyDefinition." + id;

        var def = gameContext.LookupDirector.CurrencyList.FindCurrencyByReferenceId(id);
        sceneContext.PlayerState._model.SetCurrencyAndAmountEverCollected(def.toICurrency(), amount,
            amountEverCollected);
        return true;
    }

    public static bool SetCurrencyEverCollected(string referenceID, int amountEverCollected)
    {
        if (string.IsNullOrWhiteSpace(referenceID)) return false;
        if (!inGame) return false;
        var id = referenceID;
        if (!id.StartsWith("CurrencyDefinition.")) id = "CurrencyDefinition." + id;

        var def = gameContext.LookupDirector.CurrencyList.FindCurrencyByReferenceId(id);
        sceneContext.PlayerState._model.SetCurrencyAndAmountEverCollected(def.toICurrency(),
            GetCurrency(referenceID), amountEverCollected);
        return true;
    }

    public static bool AddCurrency(string referenceID, int amount)
    {
        if (string.IsNullOrWhiteSpace(referenceID)) return false;
        if (!inGame) return false;
        var id = referenceID;
        if (!id.StartsWith("CurrencyDefinition.")) id = "CurrencyDefinition." + id;

        var def = gameContext.LookupDirector.CurrencyList.FindCurrencyByReferenceId(id);
        sceneContext.PlayerState._model.AddCurrency(def.toICurrency(), amount);
        return true;
    }

    public static int GetCurrency(string referenceID)
    {
        if (string.IsNullOrWhiteSpace(referenceID)) return -1;
        if (!inGame) return -1;
        var id = referenceID;
        if (!id.StartsWith("CurrencyDefinition.")) id = "CurrencyDefinition." + id;

        var def = gameContext.LookupDirector.CurrencyList.FindCurrencyByReferenceId(id);
        var curr = sceneContext.PlayerState._model.GetCurrencyAmount(def.toICurrency());
        if (curr.ToString() == "NaN") return 0;
        return curr;
    }

    public static int GetCurrencyEverCollected(string referenceID)
    {
        if (string.IsNullOrWhiteSpace(referenceID)) return -1;
        if (!inGame) return -1;
        var id = referenceID;
        if (!id.StartsWith("CurrencyDefinition.")) id = "CurrencyDefinition." + id;

        var def = gameContext.LookupDirector.CurrencyList.FindCurrencyByReferenceId(id);
        var curr = sceneContext.PlayerState._model.GetCurrencyAmountEverCollected(def.toICurrency());
        if (curr.ToString() == "NaN") return 0;
        return curr;
    }
}