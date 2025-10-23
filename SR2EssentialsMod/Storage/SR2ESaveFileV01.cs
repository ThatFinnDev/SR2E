using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SR2E.Storage;

public class SR2ESaveFileV01
{
    public int saveFileVersion = 1;
    public const string Extension = ".sr2save";
    public string stamp = "2021010109302";
    public string SR2ECodeVersion = null; //BuildInfo.CodeVersion;
    public string SR2EDisplayVersion = null; //BuildInfo.DisplayVersion;
    public int latest = -1;
    public Dictionary<string, object> modifiers = new Dictionary<string, object>();
    public int metaSaveSlotIndex = -1; //0;
    public string metaDisplayName = null; // = "1";
    public string metaGameName = null; //"20210101093021_1";
    public string metaGameIcon = null; //"gameIcon_battySlime";
    public ulong metaLatestSaveNumber = 0; //120;
    public bool metaFeralEnabled = true;
    public bool metaTarrEnabled = true;
    public string metaSR2Version = null; //"1.0.2 [20250930224232-103839]"; "0.1.2 [202210311641-82086]";
    public Dictionary<int,byte[]> savesData = new Dictionary<int,byte[]>();
    
    public bool IsValid()
    {
        if(string.IsNullOrWhiteSpace(stamp)) return false;
        if(string.IsNullOrWhiteSpace(SR2ECodeVersion)) return false;
        if(string.IsNullOrWhiteSpace(SR2EDisplayVersion)) return false;
        if (!stamp.All(char.IsDigit)) return false;
        if(savesData==null) return false;
        if (!savesData.ContainsKey(latest)) return false;
        foreach (var pair in savesData)
        {
            if (pair.Key < 0) return false;
            if (pair.Value==null||pair.Value.Length==0) return false;
        }
        return true;
    }
    public SR2ESaveFileV01() {}
    public SR2ESaveFileV01(Dictionary<int, byte[]> savesData, string stamp, int latest)
    {
        this.savesData = new Dictionary<int, byte[]>(savesData);
        this.stamp = stamp;
        this.latest = latest;
    }
    public static SR2ESaveFileV01 LoadJson(string jsonOrJsonPath, bool isPath)
    {
        try
        {
            if (isPath) jsonOrJsonPath = File.ReadAllText(jsonOrJsonPath);
            return JsonConvert.DeserializeObject<SR2ESaveFileV01>(jsonOrJsonPath, jsonSerializerSettings);
        }
        catch { }
        return null;
    }

    private static readonly byte[] GzipHeader = { 0x1F, 0x8B };
    public static SR2ESaveFileV01 Load(byte[] jsonOrCompressed)
    {
        if (jsonOrCompressed == null || jsonOrCompressed.Length == 0) return null;

        bool isCompressed = jsonOrCompressed.Length > 2 &&jsonOrCompressed[0] == GzipHeader[0] && jsonOrCompressed[1] == GzipHeader[1];
        if (!isCompressed) return LoadJson(Encoding.UTF8.GetString(jsonOrCompressed), false);
        MemoryStream ms = new MemoryStream(jsonOrCompressed);
        GZipStream gzip = null;
        try
        {
            gzip = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream decompressed = new MemoryStream();
            byte[] buffer = new byte[4096];
            int read;

            while ((read = gzip.Read(buffer, 0, buffer.Length)) > 0) 
                decompressed.Write(buffer, 0, read);

            byte[] result = decompressed.ToArray();
            decompressed.Dispose();
            return Load(result);
        }
        catch { return null; }
        finally
        {
            if (gzip != null) gzip.Dispose();
            ms.Dispose();
        }
    }
    public static SR2ESaveFileV01 LoadCompressed(byte[] json)
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
    public byte[] ExportCompressed()
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
            MemoryStream output = new MemoryStream();
            GZipStream gzip = null;
            try
            {
                gzip = new GZipStream(output, CompressionMode.Compress, true);
                gzip.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                if (gzip != null)
                    gzip.Dispose();
            }

            byte[] compressed = output.ToArray();
            output.Dispose();
            return compressed;
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