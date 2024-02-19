using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityEngine;
using YooAsset;

public class ResourceComponent : BaseGameComponent
{
    /// <summary>
    /// 资源包名称。
    /// </summary>
    public string PackageName = "DefaultPackage";

    /// <summary>
    /// 资源系统运行模式。
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    /// <summary>
    /// 下载文件校验等级。
    /// </summary>
    public EVerifyLevel VerifyLevel = EVerifyLevel.Middle;

    /// <summary>
    /// 设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
    /// </summary>
    [SerializeField]
    public long Milliseconds = 30;
    /// <summary>
    /// 当前最新的包裹版本。（资源版本）
    /// </summary>
    [HideInInspector]
    public string PackageVersion;
    //游戏版本
    public int AppVersion;
    public string VersionType;
    public string ResourceSourceUrl;
    public string BackUpResourceSourceUrl;

    private IResourceManager m_ResourceManager;

    private void Start()
    {
        m_ResourceManager = new ResourceManager();

#if UNITY_EDITOR
        PlayMode = EPlayMode.EditorSimulateMode;
#else
        if(PlayMode== EPlayMode.EditorSimulateMode)
        {
            PlayMode = EPlayMode.OfflinePlayMode;
        }
#endif

        m_ResourceManager.PlayMode = PlayMode;
        m_ResourceManager.VerifyLevel = VerifyLevel;
        m_ResourceManager.Milliseconds = Milliseconds;
        m_ResourceManager.InstanceRoot = transform;

        m_ResourceManager.Initialize();
    }

    public UniTask InitPackage()
    {
        return m_ResourceManager.InitPackages();
    }

    public void LoadRawFileAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        m_ResourceManager.LoadRawFileAsync(assetName, loadAssetCallbacks, userData);
    }
    public RawFileHandle LoadRawFileAsync(string assetName)
    {
        return m_ResourceManager.LoadRawFileAsync(assetName);
    }
    public string GetPackageSandboxRootDirectory(string packageName)
    {
        return m_ResourceManager.GetPackageSandboxPackageRootDirectory(packageName);
    }
    private void OnDestroy()
    {
        m_ResourceManager.Shutdown();
    }

    public UpdatePackageVersionOperation UpdatePackageVersionAsync(bool appendTimeTicks = true, int timeout = 60)
    {
        var package = YooAssets.GetPackage(PackageName);
        return package.UpdatePackageVersionAsync(appendTimeTicks, timeout);
    }

    public UpdatePackageManifestOperation UpdatePackageManifestAsync(string packageVersion, bool autoSaveVersion = true, int timeout = 60)
    {
        var package = YooAssets.GetPackage(PackageName);
        return package.UpdatePackageManifestAsync(packageVersion, autoSaveVersion, timeout);
    }
    /// <summary>
    /// 资源下载器，用于下载当前资源版本所有的资源包文件。
    /// </summary>
    public ResourceDownloaderOperation Downloader { get; set; }
    /// <summary>
    /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
    /// </summary>
    /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
    /// <param name="failedTryAgain">下载失败的重试次数</param>
    public ResourceDownloaderOperation CreateResourceDownloader(int downloadingMaxNumber, int failedTryAgain)
    {
        var package = YooAssets.GetPackage(PackageName);
        Downloader = package.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain);
        return Downloader;
    }

    /// <summary>
    /// 清理包裹未使用的缓存文件
    /// </summary>
    public ClearUnusedCacheFilesOperation ClearUnusedCacheFilesAsync()
    {
        var package = YooAssets.GetPackage(PackageName);
        return package.ClearUnusedCacheFilesAsync();
    }
}
