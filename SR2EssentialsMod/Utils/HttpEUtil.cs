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
    public static void DownloadTexture2DAsync(string url, Action<Texture2D, string> onComplete)
    {
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, onComplete));
    }

    public static void DownloadTexture2DIntoImageAsync(string url, Image image)
    {
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, (Action<Texture2D, string>)((texture, error) =>
        {
            if (error == null && texture != null)
                image.sprite = texture.Texture2DToSprite();
        })));
    }
    public static void DownloadTexture2DIntoRawImageAsync(string url, RawImage image)
    {
        MelonCoroutines.Start(_DownloadTexture2DCoroutine(url, (Action<Texture2D, string>)((texture, error) =>
        {
            if (error == null && texture != null)
                image.texture = texture;
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
