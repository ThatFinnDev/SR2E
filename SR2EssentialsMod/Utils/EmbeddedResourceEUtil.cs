using System.Diagnostics;
using System.Linq;

namespace SR2E.Utils;

public static class EmbeddedResourceEUtil
{
    public static Sprite LoadSprite(string fileName) => ConvertEUtil.Texture2DToSprite(LoadTexture2D(fileName));
    public static Texture2D LoadTexture2D(string filename) => LoadTexture2D(filename, 1);
    internal static Texture2D LoadTexture2D(string filename, int methodToGetAssembly)
    {
        var realFilename = filename.Replace("/",".");
        if (!(realFilename.EndsWith(".png") || realFilename.EndsWith(".jpg") || realFilename.EndsWith(".exr"))) return null;
        
        var method = new StackTrace().GetFrame(methodToGetAssembly).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        
        System.IO.Stream manifestResourceStream =
            assembly.GetManifestResourceStream(assembly.GetName().Name + "." + realFilename);
        
        byte[] array = new byte[manifestResourceStream.Length];
        
        manifestResourceStream.Read(array, 0, array.Length);
        
        Texture2D texture2D = new Texture2D(1, 1);
        ImageConversion.LoadImage(texture2D, array);
        
        texture2D.filterMode = FilterMode.Bilinear;
        
        return texture2D;
    }

    public static Dictionary<string, byte[]> LoadResources(string folderNamespace, bool recursive = false)
    {
        folderNamespace=folderNamespace.Replace("/",".");
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        var baseNamespace = assembly.GetName().Name + "." + folderNamespace;

        var resourceNames = assembly.GetManifestResourceNames().Where(r =>
                recursive
                    ? r.StartsWith(baseNamespace + ".")
                    : r.Substring(0, r.LastIndexOf('.')).Equals(baseNamespace))
            .ToArray();
        var result = new Dictionary<string, byte[]>();

        foreach (var resourceName in resourceNames)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) continue;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                string fileName = resourceName.Substring(baseNamespace.Length + 1);
                result[fileName] = bytes;
            }
        }

        return result;
    }

    public static byte[] LoadResource(string filename)
    {
        filename=filename.Replace("/",".");
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        System.IO.Stream manifestResourceStream =
            assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] array = new byte[manifestResourceStream.Length];
        manifestResourceStream.Read(array, 0, array.Length);
        return array;
    }
    public static string LoadString(string filename)
    {
        filename=filename.Replace("/",".");
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        System.IO.Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] buffer = new byte[16 * 1024];
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            ms.Write(buffer, 0, read);
        return System.Text.Encoding.Default.GetString(ms.ToArray());

    }
}