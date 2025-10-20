using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SR2E.Storage;

public class SR2ESaveFileV01
{
    public bool IsValid()
    {
        if(string.IsNullOrWhiteSpace(stamp)) return false;
        if (!stamp.All(char.IsDigit)) return false;
        foreach (var pair in savesData)
        {
            if (pair.Key < 0) return false;
            if (pair.Value==null||pair.Value.Length==0) return false;
        }
        return true;
    }
    public const string Extension = ".sr2save";
    public Dictionary<int,byte[]> savesData = new Dictionary<int,byte[]>();
    public int metaSaveSlotIndex = -1; //0;
    public string metaDisplayName = null; // = "1";
    public string metaGameName = null; //"20210101093021_1";
    public string stamp = "2021010109302";

    public SR2ESaveFileV01() {}
    public SR2ESaveFileV01(Dictionary<int, byte[]> savesData, string stamp)
    {
        this.savesData = new Dictionary<int, byte[]>(savesData);
        this.stamp = stamp;
    }
    public static SR2ESaveFileV01 Load(string jsonOrPath, bool isPath)
    {
        try
        {
            if (isPath) jsonOrPath = File.ReadAllText(jsonOrPath);
            return JsonConvert.DeserializeObject<SR2ESaveFileV01>(jsonOrPath, jsonSerializerSettings);
        }
        catch { }
        return null;
    }
    public static SR2ESaveFileV01 Load(byte[] json)
    {
        try
        {
            return JsonConvert.DeserializeObject<SR2ESaveFileV01>(Encoding.UTF8.GetString(json), jsonSerializerSettings);
        }
        catch { }
        return null;
    }

    public string Export()
    {
        try
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        catch { }
        return null;
    }
    public byte[] ExportBytes()
    {
        try
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        catch { }
        return null;
    }
    
    static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        Error = (sender, args) =>
        {
            if(DebugLogging.HasFlag()) MelonLogger.Msg($"Error: {args.ErrorContext.Error.Message}");
            //args.ErrorContext.Handled = false;
        }
    };
}