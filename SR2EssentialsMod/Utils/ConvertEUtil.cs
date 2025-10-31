namespace SR2E.Utils;

public static class ConvertEUtil
{
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
    public static string Texture2DToBase64PNG(Texture2D texture)
    {
        if (texture==null) return null;
        return System.Convert.ToBase64String(texture.EncodeToPNG());
    }
}