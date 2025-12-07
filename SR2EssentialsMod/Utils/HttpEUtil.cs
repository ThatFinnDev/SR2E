using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SR2E.Utils;

public static class HttpEUtil
{
    static Dictionary<Image,string> onGoingImages = new ();
    static Dictionary<RawImage,string> onGoingRawImages = new ();
    public static void DownloadTexture2DAsync(string url, Action<Texture2D, string> onComplete)
    {
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, onComplete));
    }

    public static void DownloadTexture2DIntoImageAsync(string url, Image image)
    {
        onGoingImages[image]=url;
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, (Action<Texture2D, string>)((texture, error) =>
        {
            if (error == null && texture != null)
                if(onGoingImages[image]==url)
                    image.sprite = texture.Texture2DToSprite();
        })));
        if(onGoingImages[image]==url) onGoingImages.Remove(image);
    }
    public static void DownloadTexture2DIntoRawImageAsync(string url, RawImage image)
    {
        onGoingRawImages[image]=url;
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, (Action<Texture2D, string>)((texture, error) =>
        {
            if (error == null && texture != null)
                if(onGoingRawImages[image]==url)
                    image.texture = texture;
        })));
        if(onGoingRawImages[image]==url) onGoingRawImages.Remove(image);
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
