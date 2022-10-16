using System;
using System.Collections;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
    private AssetBundle _assetBundle;
#if UNITY_ANDROID
    private string bundleURL = "https://firebasestorage.googleapis.com/v0/b/circle-fall.appspot.com/o/background_android?alt=media&token=af599529-c97b-4e49-9d80-59f951cd5853";
#endif
#if UNITY_IOS
    private string bundleURL = "https://firebasestorage.googleapis.com/v0/b/circle-fall.appspot.com/o/background_ios?alt=media&token=aa1abe31-b15f-4392-ab68-671d9f813224";
#endif
#if UNITY_STANDALONE
    private string bundleURL = "https://firebasestorage.googleapis.com/v0/b/circle-fall.appspot.com/o/background?alt=media&token=47109473-a0b3-4917-9aef-6235acea3481";
#endif
    private int version = 0;
    private Action<Sprite> onBackgroundLoaded;

    private static BundleLoader mInstance;
    public static BundleLoader Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GameObject().AddComponent<BundleLoader>();
                DontDestroyOnLoad(mInstance);
            }
            return mInstance;
        }
    }

    public void Download(int level, Action<Sprite> callback)
    {
        onBackgroundLoaded = callback;
        StartCoroutine(DownloadAndCache(level));
    }

    private IEnumerator DownloadAndCache(int level)
    {

        if (_assetBundle != null)
        {
            yield return GetSprite(_assetBundle, level);
            yield break;
        }

        WWW www = WWW.LoadFromCacheOrDownload(bundleURL, 1);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield return null;
        }

        _assetBundle = www.assetBundle;
        yield return GetSprite(_assetBundle, level);
    }

    private IEnumerator GetSprite(AssetBundle bundle, int level)
    {
        AssetBundleRequest spriteRequest;
        spriteRequest = _assetBundle.LoadAssetAsync($"bg_{level}", typeof(Sprite));
        yield return spriteRequest;
        onBackgroundLoaded?.Invoke(spriteRequest.asset as Sprite);
        onBackgroundLoaded = null;
    }
}
