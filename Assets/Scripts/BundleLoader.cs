using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
#if UNITY_ANDROID
    private string bundleURL = "https://firebasestorage.googleapis.com/v0/b/circle-fall.appspot.com/o/background_android?alt=media&token=af599529-c97b-4e49-9d80-59f951cd5853";
#endif
#if UNITY_IOS
    private string bundleURL = "https://firebasestorage.googleapis.com/v0/b/circle-fall.appspot.com/o/background_ios?alt=media&token=aa1abe31-b15f-4392-ab68-671d9f813224";
#endif
#if UNITY_STANDALONE
    private string bundleURL = "https://firebasestorage.googleapis.com/v0/b/circle-fall.appspot.com/o/background?alt=media&token=47109473-a0b3-4917-9aef-6235acea3481";
#endif
    private const int Version = 0;
    private AssetBundle _assetBundle;
    private Action<Sprite> _onBackgroundLoaded;

    private static BundleLoader _instance;
    public static BundleLoader Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(_instance);
    }

    public async UniTask Download(int level, CancellationTokenSource cts, Action<Sprite> callback)
    {
        _onBackgroundLoaded = callback;
        if(cts.Token.IsCancellationRequested) return;
        await DownloadAndCache(level, cts);
    }

    private async UniTask DownloadAndCache(int level, CancellationTokenSource cts)
    {
        if (_assetBundle != null)
        {
            await GetSprite(level, cts);
            cts.Cancel();
        }
        if(cts.IsCancellationRequested) return;
        
        var www = WWW.LoadFromCacheOrDownload(bundleURL, Version);
        await www;

        _assetBundle = www.assetBundle;
         await GetSprite(level, cts);
    }

    private async UniTask GetSprite(int level, CancellationTokenSource cts)
    {
        if(cts.IsCancellationRequested) return;
        var spriteRequest = _assetBundle.LoadAssetAsync($"bg_{level}", typeof(Sprite));
        await spriteRequest;
        _onBackgroundLoaded?.Invoke(spriteRequest.asset as Sprite);
        _onBackgroundLoaded = null;
    }
}
