using System.Security.Cryptography;
using System.Text;

namespace SR2E.Utils;

internal static class EncodingEUtil
{
    internal static string CreateMD5(this string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("x2"));
            return sb.ToString();
        }
    }
    internal static string EncodeToBase128(this byte[] data)
    {
        var result = new StringBuilder((data.Length * 8 + 6) / 7);
        int storage = 0;
        int bits = 0;

        foreach (byte b in data)
        {
            storage = (storage << 8) | b;
            bits += 8;
            while (bits >= 7)
            {
                bits -= 7;
                result.Append((char)((storage >> bits) & 0x7F));
            }
        }

        if (bits > 0) 
        {
            result.Append((char)((storage << (7 - bits)) & 0x7F));
        }

        return result.ToString();
    }

    internal static byte[] DecodeFromBase128(this string data)
    {
        var result = new List<byte>();
        int storage = 0;
        int bits = 0;

        foreach (char c in data)
        {
            storage = (storage << 7) | (c & 0x7F);
            bits += 7;
            if (bits >= 8)
            {
                bits -= 8;
                result.Add((byte)((storage >> bits) & 0xFF));
            }
        }
        return result.ToArray();
    }
}