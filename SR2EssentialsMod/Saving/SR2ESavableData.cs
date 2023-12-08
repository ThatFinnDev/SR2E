
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SR2E.Saving;



[Serializable]
internal class SR2ESavableData
{

    public static string currPath;
    public string dir;
    public string gameName;
    public int idx;

    public static SR2ESavableData Instance;
    public SR2ESavableData()
    {
        Instance = this;
        gordoSavedData = new Dictionary<string, SR2EGordoData>();
        slimeSavedData = new Dictionary<long, SR2ESlimeData>();
    }
    

    public void IncreaseSaveIndex()
    {
        if (idx != AutoSaveDirector.MAX_AUTOSAVES)
        {
            currPath = $"{Path.Combine(dir, gameName)}_{idx + 1}.sr2e";
            idx++;
        }
        else
        {
            currPath = $"{Path.Combine(dir, gameName)}_{0}.sr2e";
            idx = 0;
        }
    }


    /// <summary>
    /// Gordo ID to Gordo Data
    /// </summary>
    public Dictionary<string, SR2EGordoData> gordoSavedData;

    /// <summary>
    /// Actor ID to Slime Data
    /// </summary>
    public Dictionary<long, SR2ESlimeData> slimeSavedData;

    public SR2EPlayerData playerSavedData = new SR2EPlayerData();

    public void SaveToStream(Stream stream)
    {
        var writer = new StreamWriter(stream);

        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        writer.Write(json);
    }

    public void DebugSaveToNewFile(string path)
    {
        try
        {
            var json = DebugPrint();
            if (SR2EEntryPoint.debugLogging)
                MelonLogger.Msg(json);
            using (var writer = new StreamWriter(path))
            {
                writer.Write(json);
                writer.Flush();
            }
        }
        catch (Exception error)
        {
            SR2Console.SendError($"Saving error: {error}");
        }

    }

    public string DebugPrint() => JsonConvert.SerializeObject(this, Formatting.Indented);

    public void TrySave()
    {

        if (Instance.playerSavedData.vacMode == VacModes.AUTO_VAC || Instance.playerSavedData.vacMode == VacModes.AUTO_VAC)
        {
            Instance.playerSavedData.vacMode = VacModes.NORMAL;
        }
        //var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        //File.WriteAllText(currPath, json);
        DebugSaveToNewFile(currPath);
    }
    public static SR2ESavableData LoadFromStream(Stream stream)
    {
        using (var localStream = stream)
        {
            var reader = new StreamReader(localStream);
            var json = reader.ReadToEnd();
            SR2ESavableData save = JsonConvert.DeserializeObject<SR2ESavableData>(json);
            if (Instance.playerSavedData.vacMode == VacModes.AUTO_VAC || Instance.playerSavedData.vacMode == VacModes.AUTO_VAC)
            {
                Instance.playerSavedData.vacMode = VacModes.NORMAL;
            }
            return save;
        }
    }
}
