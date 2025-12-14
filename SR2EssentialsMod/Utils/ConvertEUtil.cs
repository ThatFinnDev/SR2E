using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Economy;

namespace SR2E.Utils;

public static class ConvertEUtil
{
    public static ISaveReferenceTranslation toIVariant(this SaveReferenceTranslation saveReferenceTranslation) => saveReferenceTranslation.TryCast<ISaveReferenceTranslation>();
    public static SaveReferenceTranslation toNonIVariant(this ISaveReferenceTranslation iSaveReferenceTranslation) => iSaveReferenceTranslation.TryCast<SaveReferenceTranslation>();
    public static ILoadReferenceTranslation toIVariant(this LoadReferenceTranslation saveReferenceTranslation) => saveReferenceTranslation.TryCast<ILoadReferenceTranslation>();
    public static LoadReferenceTranslation toNonIVariant(this ILoadReferenceTranslation saveReferenceTranslation) => saveReferenceTranslation.TryCast<LoadReferenceTranslation>();

    public static Sprite Texture2DToSprite(this Texture2D texture)
    {
        if (texture == null) return null;
        return Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);
    }
    public static Texture2D Base64ToTexture2D(string base64)
    {
        if (string.IsNullOrEmpty(base64)) return null;
        byte[] bytes = System.Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(bytes, false)) return texture;
        return null;
    }
    public static Texture2D BytesToTexture2D(byte[] bytes)
    {
        if (bytes==null) return null;
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(bytes, false)) return texture;
        return null;
    }
    public static byte[] Texture2DToBytesPNG(Texture2D texture)
    {
        if (texture==null) return null;
        return Il2CppImageConversionManager.EncodeToPNG(texture);
    }
    public static byte[] Texture2DToBytesJPG(Texture2D texture)
    {
        if (texture==null) return null;
        return Il2CppImageConversionManager.EncodeToJPG(texture);
    }
    public static byte[] Texture2DToBytesTGA(Texture2D texture)
    {
        if (texture==null) return null;
        return Il2CppImageConversionManager.EncodeToTGA(texture);
    }
    public static string Texture2DToBase64PNG(Texture2D texture)
    {
        if (texture==null) return null;
        return System.Convert.ToBase64String(Il2CppImageConversionManager.EncodeToPNG(texture));
    }
    public static string Texture2DToBase64JPG(Texture2D texture)
    {
        if (texture==null) return null;
        return System.Convert.ToBase64String(Il2CppImageConversionManager.EncodeToJPG(texture));
    }
    public static string Texture2DToBase64TGA(Texture2D texture)
    {
        if (texture==null) return null;
        return System.Convert.ToBase64String(Il2CppImageConversionManager.EncodeToTGA(texture));
    }
}
