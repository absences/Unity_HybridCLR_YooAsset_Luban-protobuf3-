using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using System.IO;
using TMPro;
using UnityEngine;
using YooAsset;

[System.Serializable]
public class ActivePackage
{
    public string packageVersion;//当前资源版本
    public string prepackageVersion;//预下载资源版本
    public ResourcePackage package;
}

public class ResourceComponent : BaseGameComponent
{
    /// <summary>
    /// 资源系统运行模式。
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    /// <summary>
    /// 设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
    /// </summary>
    public long Milliseconds = 30;

    public ActivePackage rawfilePackage;
    public ActivePackage assetPackage;


    public string rawfilePackageName = "rawfilePackage";

    public string assetPackageName = "assetPackage";
    //游戏版本
    public int AppVersion;
    public string VersionType;
    public string ResourceSourceUrl;
    public string BackUpResourceSourceUrl;

    private IResourceManager m_ResourceManager;

    private bool resourceInited = false;
    public IResourceManager ResourceManager
    {
        get
        {
            if (m_ResourceManager == null)
            {
                m_ResourceManager = new ResourceManager();

#if !UNITY_EDITOR
                 if(PlayMode== EPlayMode.EditorSimulateMode)
                 {
                     PlayMode = EPlayMode.OfflinePlayMode;
                 }
#endif
                m_ResourceManager.PlayMode = PlayMode;
                m_ResourceManager.Milliseconds = Milliseconds;
                m_ResourceManager.InstanceRoot = transform;

                m_ResourceManager.Initialize();
            }

            return m_ResourceManager;
        }
    }

    public async UniTask<bool> InitPackage()
    {
        var result = ResourceManager.InitPackages();
        resourceInited = await result;

        var package = YooAssets.GetPackage(rawfilePackageName);

        rawfilePackage = new ActivePackage()
        {
            package = package,
            packageVersion = package.GetPackageVersion()
        };
        package = YooAssets.GetPackage(assetPackageName);

        assetPackage = new ActivePackage()
        {
            package = package,
            packageVersion = package.GetPackageVersion()
        };
        return resourceInited;
    }
    public async UniTask InitGameAsset()
    {
        AssetHandle handle = LoadAssetAsync<TMP_FontAsset>(TMPAssetLoader.Tmp_Font_Asset);
        await handle;
        MaterialReferenceManager.instance.Clear();

        TMP_Settings.defaultFontAsset = handle.GetAssetObject<TMP_FontAsset>();

        handle = LoadAssetAsync<ShaderVariantCollection>("sv");

        await handle;

        var asset = handle.GetAssetObject<ShaderVariantCollection>();

        asset.WarmUp();

    }

    public AssetHandle LoadAssetAsync<T>(string location, uint priority = 0) where T : Object
    {
        AssetInfo assetInfo = GetAssetInfo<T>(location);

        return ResourceManager.LoadAssetAsync(assetInfo, priority);
    }

    public void LoadAssetAsync(AssetInfo assetInfo, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        ResourceManager.LoadAssetAsync(assetInfo, priority, loadAssetCallbacks, userData);
    }
    public void LoadAssetAsync<T>(string location, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData) where T : Object
    {
        AssetInfo assetInfo = GetAssetInfo<T>(location);

        ResourceManager.LoadAssetAsync(assetInfo, priority, loadAssetCallbacks, userData);
    }
    public AssetInfo GetAssetInfo<T>(string location)
    {
        return assetPackage.package.GetAssetInfo(location, typeof(T));
    }

    public void LoadRawFileAsync(string assetName, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        ResourceManager.LoadRawFileAsync(assetName, priority, loadAssetCallbacks, userData);
    }
    public RawFileHandle LoadRawFileAsync(string assetName, uint priority = 0)
    {
        return ResourceManager.LoadRawFileAsync(assetName, priority);
    }
    public RawFileHandle LoadRawFileSync(string assetName)
    {
        return ResourceManager.LoadRawFileSync(assetName);
    }
    public AllAssetsHandle LoadAllAssetsAsync<T>(string location, uint priority) where T : Object
    {
        return ResourceManager.LoadAllAssetsAsync<T>(location, priority);
    }

    public UpdatePrePackageVersionOperation UpdatePrePackageVersionAsync(string packageName)
    {
        return ResourceManager.UpdatePrePackageVersionAsync(packageName);
    }

    public PreDownloadContentOperation PreDownloadContentAsync(string packageName, string version)
    {
        return ResourceManager.PreDownloadContentAsync(packageName, version);
    }
    /// <summary>
    /// 加载子资源对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="onsuccess"></param>
    /// <param name="onFail"></param>
    /// <exception cref="GameFrameworkException"></exception>
    public void LoadSubAssetsAsync<T>(string location, string subName, System.Action<SubAssetsHandle, T> onsuccess, System.Action onFail) where T : Object
    {
        if (string.IsNullOrEmpty(location))
        {
            throw new GameFrameworkException("location is invalid.");
        }
        AssetInfo assetInfo = GetAssetInfo<T>(location);
        if (!string.IsNullOrEmpty(assetInfo.Error))
        {
            onFail();
            Log.Error(assetInfo.Error);
            return;
        }

        m_ResourceManager.LoadSubAssetsAsync(assetInfo, 0).Completed += (handle) =>
        {
            if (handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = string.Format("Can not load asset '{0}'. {1}", assetInfo.AssetPath, assetInfo.Error);

                throw new GameFrameworkException(errorMessage);
            }
            else if (handle.Status == EOperationStatus.Succeed)
            {
                onsuccess?.Invoke(handle, handle.GetSubAssetObject<T>(subName));
            }
            else
            {
                onFail();
            }
        };
    }


    public void UnloadAsset(HandleBase assetHandle)
    {
        if (assetHandle is AssetHandle asset)
            asset.Release();
        else if (assetHandle is RawFileHandle raw)
            raw.Release();
    }

    public string GetPackageSandboxRootDirectory(string packageName)
    {
        return m_ResourceManager.GetPackageSandboxPackageRootDirectory(packageName);
    }
    void Update()
    {
        if (resourceInited)
            m_ResourceManager.Update(Time.deltaTime, Time.realtimeSinceStartup);
    }
    private void OnDestroy()
    {
        m_ResourceManager.Shutdown();
    }
    public string GetPackageVersion(string packageName)
    {
        var package = YooAssets.GetPackage(packageName);
        return package.GetPackageVersion();
    }
    public UpdatePackageVersionOperation UpdatePackageVersionAsync(string packageName, bool appendTimeTicks = true, int timeout = 60)
    {
        ResourcePackage package = YooAssets.GetPackage(packageName);
        return package.UpdatePackageVersionAsync(appendTimeTicks, timeout);
    }
    public UpdatePackageManifestOperation UpdatePackageManifestAsync(string packageName, string packageVersion, bool autoSaveVersion = true, int timeout = 60)
    {
        var package = YooAssets.GetPackage(packageName);

        return package.UpdatePackageManifestAsync(packageVersion, autoSaveVersion, timeout);
    }

    /// <summary>
    /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
    /// </summary>
    /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
    /// <param name="failedTryAgain">下载失败的重试次数</param>
    public ResourceDownloaderOperation CreateResourceDownloader(string packageName, int downloadingMaxNumber, int failedTryAgain)
    {
        var package = YooAssets.GetPackage(packageName);
        return package.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain);
    }
    /// <summary>
    /// 清理包裹未使用的缓存文件
    /// </summary>
    public ClearUnusedCacheFilesOperation ClearUnusedCacheFilesAsync(string packageName)
    {
        var package = YooAssets.GetPackage(packageName);
        return package.ClearUnusedCacheFilesAsync();
    }
    public void SaveVersion(string packageName)
    {
        var package = YooAssets.GetPackage(packageName);
        package.SaveManifestVersionFile();
    }
    /// <summary>
    /// 清理沙盒目录
    /// </summary>
    public void ClearSandbox()
    {
        var directoryPath = GetPackageSandboxRootDirectory(assetPackageName);

        if (Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, true);

        directoryPath = GetPackageSandboxRootDirectory(rawfilePackageName);

        if (Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, true);

    }
}
