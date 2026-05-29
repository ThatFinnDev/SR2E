/*using System.Text;

namespace Starlight.EncodedAssets;

internal static class AssetEncoder
{
    internal static void ConvertAssetToCSharpFile(string inputFilePath, string outputCsPath)
    {
        var bytes = File.ReadAllBytes(inputFilePath);
        var sb = new StringBuilder();
        
        sb.AppendLine("#pragma warning disable CS8618");
        sb.AppendLine("#pragma warning disable IDE0055");
        sb.AppendLine("namespace Starlight.EncodedAssets {");
        sb.AppendLine("    public static class TheAssetName {");
        sb.Append("        [System.CodeDom.Compiler.GeneratedCode(\"AssetPacker\", \"1.0\")]");
        sb.AppendLine("        public static readonly byte[] RawAssetBytes = new byte[] {");

        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append($"0x{bytes[i]:X2},");
            if (i % 16 == 15) sb.AppendLine();
        }

        sb.AppendLine("\n        };");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        File.WriteAllText(outputCsPath, sb.ToString());
    }
}*/