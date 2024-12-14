using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.General;
//Experimental
[HarmonyPatch(typeof(SavedGame), nameof(SavedGame.Push), typeof(GameModel))]
public static class SavedGamePatch
{
	public static void Prefix(SavedGame __instance)
	{
		if (SR2EEntryPoint.fixSaves)
		{
			FixActors(__instance);
			FixPedia(__instance);
			FixLandPlots(__instance);
			FixWeatherPatterns(__instance);
			FixWeatherStates(__instance);
		}
	}

	public static void FixActors(SavedGame savedGame)
	{
		//Remove invalid Actors
		foreach (ActorDataV02 actor in savedGame.gameState.Actors)
		{
			IdentifiableType identifiableType = savedGame.persistenceIdToIdentifiableType.GetIdentifiableType(actor.TypeId);
			if (identifiableType == null)
				savedGame.gameState.Actors.Remove(actor);
		}
	}
	public static void FixPedia(SavedGame savedGame)
	{
		//Remove invalid unlocked pedia entries
		foreach (string unlockedID in savedGame.gameState.Pedia.UnlockedIds)
			if (!savedGame.pediaEntryLookup.ContainsKey(unlockedID))
				savedGame.gameState.Pedia.UnlockedIds.Remove(unlockedID);
	}
	public static void FixLandPlots(SavedGame savedGame)
	{
		foreach(LandPlotV02 plot in savedGame.gameState.Ranch.Plots)
		{
			//Remove invalid plot
			if (!Enum.IsDefined<LandPlot.Id>(plot.TypeId))
				savedGame.gameState.Ranch.Plots.Remove(plot);
			else
				foreach (LandPlot.Upgrade upgrade in plot.Upgrades)
					if (!Enum.IsDefined<LandPlot.Upgrade>(upgrade))
						//Remove invalid upgrade
						plot.Upgrades.Remove(upgrade);
		}
	}

	public static void FixWeatherPatterns(SavedGame savedGame)
	{
		var reverseLookup = savedGame._weatherPatternTranslation.ReverseLookupTable;
		var lookup = savedGame._weatherPatternTranslation.InstanceLookupTable;
		List<string> table = new List<string>();
		if (reverseLookup._indexTable != null)
		{
			table = reverseLookup._indexTable.ToList();
			var stateDict = savedGame._weatherPatternTranslation.RawLookupDictionary;
			//Remove invalid weather pattern
			foreach (var index in reverseLookup._indexTable)
				if (!stateDict.ContainsKey(index))
					table.Remove(index);
		}
		foreach (var entry in savedGame.gameState.Weather.Entries)
		{
			// Remove invalid completion time ids
			foreach (var id in entry.PatternCompletionTimeIDs)
				if (!lookup._reverseIndex.ContainsValue(id))
					entry.PatternCompletionTimeIDs.Remove(id);
			// Remove invalid forecasts
			foreach (var forecastEntry in entry.Forecast)
				if (!lookup._reverseIndex.ContainsValue(forecastEntry.PatternID))
					entry.Forecast.Remove(forecastEntry);
		}
		//Apply
		reverseLookup._indexTable = table.ToArray();
		savedGame.gameState.WeatherIndex.PatternIndexTable = table.ToArray();
	}
	public static void FixWeatherStates(SavedGame savedGame)
	{
		var reverseLookup = savedGame._weatherStateTranslation.ReverseLookupTable;
		var lookup = savedGame._weatherStateTranslation.InstanceLookupTable;

		List<string> table = new List<string>();
		if (reverseLookup._indexTable != null)
		{
			table = reverseLookup._indexTable.ToList();
			var stateDict = savedGame._weatherStateTranslation.RawLookupDictionary;
			//Remove invalid weather state
			foreach (var index in reverseLookup._indexTable)
				if (!stateDict.ContainsKey(index))
					table.Remove(index);
		}
		foreach (var entry in savedGame.gameState.Weather.Entries)
		{
			// Remove invalid completion time ids
			foreach (var id in entry.StateCompletionTimeIDs)
				if (!lookup._reverseIndex.ContainsValue(id))
					entry.StateCompletionTimeIDs.Remove(id);
			// Remove invalid forecasts
			foreach (var forecastEntry in entry.Forecast)
				if (!lookup._reverseIndex.ContainsValue(forecastEntry.StateID))
					entry.Forecast.Remove(forecastEntry);
		}
		//Apply
		reverseLookup._indexTable = table.ToArray();
		savedGame.gameState.WeatherIndex.StateIndexTable = table.ToArray();
	}
}