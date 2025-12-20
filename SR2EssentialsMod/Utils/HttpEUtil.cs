using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SR2E.Utils;

public static class HttpEUtil
{
    static Dictionary<Image,string> onGoingImages = new ();
    static Dictionary<RawImage,string> onGoingRawImages = new ();
    static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        if (newWidth <= 0 || newHeight <= 0) return source;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return newTexture;
    }
    public static void DownloadTexture2DAsync(string url, Action<Texture2D, string> onComplete)
    {
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, onComplete));
    }

    public static void DownloadTexture2DIntoImageAsync(string url, Image image, bool useCache = false, int resizeX = -1, int resizeY = -1)
    {
        onGoingImages[image]=url;
        var cachePath = Path.Combine(SR2EEntryPoint.TmpDataPath, "downloadcache."+Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(url)))+".png");
        if (useCache)
            try { image.sprite = ConvertEUtil.BytesToTexture2D(File.ReadAllBytes(cachePath)).Texture2DToSprite(); } catch { }
        
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, ((texture, error) =>
        {
            if(onGoingImages.ContainsKey(image))
                if (onGoingImages[image] == url)
                {
                    onGoingImages.Remove(image);
                    if (error == null && texture != null&&image!=null)
                    {
                        image.sprite = texture.Texture2DToSprite();
                        if (useCache)
                            File.WriteAllBytes(cachePath,ConvertEUtil.Texture2DToBytesPNG(ResizeTexture(texture,resizeX,resizeY)));
                    }
                }
        })));;
    }
    public static void DownloadTexture2DIntoRawImageAsync(string url, RawImage image, bool useCache = false, int resizeX = -1, int resizeY = -1)
    {
        onGoingRawImages[image]=url;
        var cachePath = Path.Combine(SR2EEntryPoint.TmpDataPath, "downloadcache."+Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(url)))+".png");
        if (useCache)
            try { image.texture = ConvertEUtil.BytesToTexture2D(File.ReadAllBytes(cachePath)); } catch { }
        
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, ((texture, error) =>
        {
            if(onGoingRawImages.ContainsKey(image))
                if (onGoingRawImages[image] == url)
                {
                    onGoingRawImages.Remove(image);
                    if (image != null)
                    {
                        if (error == null && texture != null) 
                            image.texture = texture;
                        if (useCache)
                            File.WriteAllBytes(cachePath,ConvertEUtil.Texture2DToBytesPNG(ResizeTexture(texture,resizeX,resizeY)));
                    }
                }
        })));
    }
    private static IEnumerator _DownloadTexture2DCoroutine(string url, Action<Texture2D, string> onComplete)
    {
        byte[] imageBytes = null;
        string error = null;

        Task downloadTask = Task.Run(async () =>
        {
            try
            {
                HttpClient client = new HttpClient();
                imageBytes = await client.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        });

        while (!downloadTask.IsCompleted)
            yield return null;

        if (!string.IsNullOrEmpty(error) || imageBytes == null)
        {
            onComplete?.Invoke(null, error ?? "Unknown error while downloading image");
            yield break;
        }

        Texture2D texture = new Texture2D(2, 2);
        bool loaded = texture.LoadImage(imageBytes,false);

        if (!loaded)
        {
            error = "Failed to load image data into texture.";
            onComplete?.Invoke(null, error);
            yield break;
        }

        onComplete?.Invoke(texture, null);
    }
}
