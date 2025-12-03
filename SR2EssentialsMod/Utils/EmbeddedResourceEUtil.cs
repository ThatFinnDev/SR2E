using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader.TinyJSON;

namespace SR2E.Utils;

public static class EmbeddedResourceEUtil
{
    public static Sprite LoadSprite(string fileName)
    {
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        return LoadSprite(fileName,assembly);
    }
    public static Sprite LoadSprite(string fileName, Assembly assembly) => ConvertEUtil.Texture2DToSprite(LoadTexture2D(fileName,assembly));
    
    
    
    public static Texture2D LoadTexture2D(string fileName)
    {
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        return LoadTexture2D(fileName, assembly);
    }
    public static Texture2D LoadTexture2D(string filename, Assembly assembly)
    {
        if (assembly == null) return null;
        var realFilename = filename.Replace("/",".");
        if (!(realFilename.EndsWith(".png") || realFilename.EndsWith(".jpg") || realFilename.EndsWith(".exr"))) return null;
        
        System.IO.Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        
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
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        return LoadResource(filename, assembly);
    }
    public static byte[] LoadResource(string filename, Assembly assembly)
    {
        if(assembly == null) return null;
        filename=filename.Replace("/",".");
        System.IO.Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        return array;
    }
    
    
    
    public static string LoadString(string filename)
    {
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        return LoadString(filename, assembly);
    }
    public static string LoadString(string filename, Assembly assembly)
    {
        if(assembly == null) return null;
        filename=filename.Replace("/",".");
        System.IO.Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        return System.Text.Encoding.Default.GetString(array);

    }
    
    public static Il2CppAssetBundle LoadIl2CppBundle(string filename)
    {
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        return LoadIl2CppBundle(filename, assembly);
    }
    public static Il2CppAssetBundle LoadIl2CppBundle(string filename, Assembly assembly)
    {
        if(assembly == null) return null;
        filename=filename.Replace("/",".");
        System.IO.Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        
        return Il2CppAssetBundleManager.LoadFromMemory(array);
    }
    
    public static AssetBundle LoadBundle(string filename)
    {
        var method = new StackTrace().GetFrame(1).GetMethod();
        var assembly = method.ReflectedType.Assembly;
        return LoadBundle(filename, assembly);
    }
    public static AssetBundle LoadBundle(string filename, Assembly assembly)
    {
        if(assembly == null) return null;
        filename=filename.Replace("/",".");
        System.IO.Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + filename);
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        
        return AssetBundle.LoadFromMemory(array);
    }
    
}