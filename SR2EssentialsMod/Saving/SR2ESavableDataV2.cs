/*using System;
using System.IO;
using Newtonsoft.Json;
using Il2CppMonomiPark.SlimeRancher.DataModel;
namespace SR2E.Saving;

[Serializable]
internal class SR2ESavableDataV2
{
    public static string currPath;
    public string dir;
    public string gameName;

    public static SR2ESavableDataV2 Instance;
    public SR2ESavableDataV2()
    {
        Instance = this;
        gordoSavedData = new Dictionary<string, SR2EGordoData>();
        slimeSavedData = new Dictionary<long, SR2ESlimeData>();
        gadgetSavedData = new Dictionary<long, SR2EGadgetData>();
    }

    public Dictionary<string, SR2EGordoData> gordoSavedData;
    public Dictionary<long, SR2ESlimeData> slimeSavedData;
    public Dictionary<long, SR2EGadgetData> gadgetSavedData;
    public SR2EPlayerData playerSavedData = new SR2EPlayerData();

    public void SaveToStream(Stream stream)
    {
        var writer = new StreamWriter(stream);
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        writer.Write(json);
    }

    public void TrySave()
    {
        try
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (SR2EEntryPoint.debugLogging) MelonLogger.Msg(json);
            using (var writer = new StreamWriter(currPath))
            {
                writer.Write(json);
                writer.Flush();
            }
        }
        catch (Exception error)
        {
            SR2EConsole.SendError($"Saving error: {error}");
        }
    }
    public static SR2ESavableDataV2 LoadFromStream(Stream stream)
    {
        using (var localStream = stream)
        {
            var reader = new StreamReader(localStream);
            var json = reader.ReadToEnd();
            SR2ESavableDataV2 save = JsonConvert.DeserializeObject<SR2ESavableDataV2>(json);
            return save;
        }
    }
}*/
//Broken as of SR2 0.6.0