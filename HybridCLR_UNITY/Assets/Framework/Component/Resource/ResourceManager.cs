using System.Threading;
using UnityEngine;
using YooAsset;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using GameFramework;

public class ResourceManager : IResourceManager
{
    public EPlayMode PlayMode { get; set; }
    public EVerifyLevel VerifyLevel { get; set; }
    public long Milliseconds { get; set; }
    public Transform InstanceRoot { get; set; }

    /// <summary>
    /// 资源生命周期服务器。
    /// </summary>
    public ResourceHelper ResourceHelper { get; private set; }

    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    public CancellationToken CancellationToken { get; private set; }

    private bool _inited = false;

    public void Initialize()
    {
        // 初始化资源系统
        YooAssets.Initialize(new YooAssetsLogger());
        YooAssets.SetOperationSystemMaxTimeSlice(Milliseconds);

        ResourceHelper = InstanceRoot.gameObject.AddComponent<ResourceHelper>();
        CancellationToken = ResourceHelper.GetCancellationTokenOnDestroy();
    }
    InitializationOperation InitPackage(string buildPipelineName, ResourcePackage package)
    {
        switch (PlayMode)
        {
            case EPlayMode.EditorSimulateMode:  // 编辑器下的模拟模式
                {
                    var createParameters = new EditorSimulateModeParameters
                    {
                        SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipelineName, package.PackageName),
                        CacheFileAppendExtension = true
                    };
                    return package.InitializeAsync(createParameters);
                }
            case EPlayMode.OfflinePlayMode:
                {
                    var createParameters = new OfflinePlayModeParameters
                    {
                        DecryptionServices = new GameDecryptionServices(),//文件解密服务
                        CacheFileAppendExtension = true
                    };
                    return package.InitializeAsync(createParameters);
                }
            case EPlayMode.HostPlayMode:
                {
                    var createParameters = new HostPlayModeParameters
                    {
                        DecryptionServices = new GameDecryptionServices(),//文件解密服务
                        DeliveryQueryServices = new DefaultDeliveryQueryServices(),//分发资源服务
                        DeliveryLoadServices = new DefaultDeliveryLoadServices(),//加载服务
                        BuildinQueryServices = new GameQueryServices(),
                        RemoteServices = new GameRemoteServices(),
                        CacheFileAppendExtension = true
                    };
                    return package.InitializeAsync(createParameters);
                }

            case EPlayMode.WebPlayMode:
                {
                    //WebGL平台的专属模式，包括微信小游戏，抖音小游戏都需要选择该模式。
                    var createParameters = new WebPlayModeParameters()
                    {
                        BuildinQueryServices = new GameQueryServices(),
                        RemoteServices = new GameRemoteServices(),
                        CacheFileAppendExtension = true
                    };
                    return package.InitializeAsync(createParameters);
                }
        }
        return null;
    }
    public async UniTask<bool> InitPackages()
    {
        var package = YooAssets.CreatePackage(GameEnter.Resource.assetPackageName);
        var op = InitPackage(EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), package);
        await op;

        YooAssets.SetDefaultPackage(package);

        package = YooAssets.CreatePackage(GameEnter.Resource.rawfilePackageName);
        var op2 = InitPackage(EDefaultBuildPipeline.RawFileBuildPipeline.ToString(), package);
        await op2;

        _inited = (op.Status == op2.Status) && op.Status == EOperationStatus.Succeed;
        return _inited;
    }

    public void LoadRawFileAsync(string assetName, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        float duration = Time.time;

        if (string.IsNullOrEmpty(assetName))
        {
            throw new GameFrameworkException("Asset name is invalid.");
        }

        if (loadAssetCallbacks == null)
        {
            throw new GameFrameworkException("Load asset callbacks is invalid.");
        }

        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);

        AssetInfo assetInfo = assetPackage.GetAssetInfo(assetName);

        assetPackage.LoadRawFileAsync(assetInfo, priority).Completed += (handle) =>
        {
            if (handle == null || handle.IsValid == false || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, EOperationStatus.Failed, errorMessage, userData);
                    return;
                }

                throw new GameFrameworkException(errorMessage);
            }
            else
            {
                if (loadAssetCallbacks.LoadAssetSuccessCallback != null)
                {
                    duration = Time.time - duration;

                    loadAssetCallbacks.LoadAssetSuccessCallback(assetName, handle, duration, userData);
                }
            }
        };
    }
    /// <summary>
    /// 异步加载原生文件
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public RawFileHandle LoadRawFileAsync(string assetName, uint priority)
    {
        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.rawfilePackageName);

        AssetInfo assetInfo = assetPackage.GetAssetInfo(assetName);

        return assetPackage.LoadRawFileAsync(assetInfo, priority);
    }

    public RawFileHandle LoadRawFileSync(string assetName)
    {
        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.rawfilePackageName);

        AssetInfo assetInfo = assetPackage.GetAssetInfo(assetName);

        return assetPackage.LoadRawFileSync(assetInfo);
    }
    public string GetPackageSandboxPackageRootDirectory(string packageName)
    {
        ResourcePackage package = YooAssets.GetPackage(packageName);

        return package.GetPackageSandboxPackageRootDirectory();
    }

    public AllAssetsHandle LoadAllAssetsAsync<T>(string location, uint priority) where T : Object
    {
        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);

        return assetPackage.LoadAllAssetsAsync<T>(location, priority);
    }
    public SubAssetsHandle LoadSubAssetsAsync(AssetInfo assetInfo, uint priority)
    {
        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);

        return assetPackage.LoadSubAssetsAsync(assetInfo, priority);
    }

    public UpdatePrePackageVersionOperation UpdatePrePackageVersionAsync(string packageName)
    {
        var assetPackage = YooAssets.TryGetPackage(packageName);

        return assetPackage.UpdatePrePackageVersionAsync();
    }

    public PreDownloadContentOperation PreDownloadContentAsync(string packageName,string version)
    {
        var assetPackage = YooAssets.TryGetPackage(packageName);

        return assetPackage.PreDownloadContentAsync(version);
    }

    public T LoadAssetSync<T>(string location) where T : Object
    {
        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);

        AssetInfo assetInfo = assetPackage.GetAssetInfo(location, typeof(T));

        var handle = assetPackage.LoadAssetSync(assetInfo);

        return handle.GetAssetObject<T>();
    }

    public AssetHandle LoadAssetAsync(AssetInfo assetInfo, uint priority)
    {
        var assetPackage = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);
       
        return assetPackage.LoadAssetAsync(assetInfo);
    }


    public void LoadAssetAsync(AssetInfo assetInfo, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        float duration = Time.time;

        if (assetInfo.IsInvalid)
        {
            throw new GameFrameworkException("Asset is invalid.");
        }

        if (loadAssetCallbacks == null)
        {
            throw new GameFrameworkException("Load asset callbacks is invalid.");
        }

        var package = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);

        package.LoadAssetAsync(assetInfo, priority).Completed += (handle) =>
        {
            if (handle == null || handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetInfo.AssetPath);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetInfo.Address, EOperationStatus.Failed, errorMessage, userData);
                    return;
                }

                throw new GameFrameworkException(errorMessage);
            }
            else
            {
                if (loadAssetCallbacks.LoadAssetSuccessCallback != null)
                {
                    duration = Time.time - duration;

                    loadAssetCallbacks.LoadAssetSuccessCallback(assetInfo.Address, handle, duration, userData);
                }
            }
        };
    }
    public void Shutdown()
    {
        YooAssets.Destroy();
    }

    private float CheckUnloadInterval = 20;
    private float CheckUnloadTime = 0;

    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        if (_inited)
        {
            if (CheckUnloadTime > CheckUnloadInterval)
            {
                DelayUnload();
                CheckUnloadTime = 0;
            }
            else
            {
                CheckUnloadTime += Time.deltaTime;
            }
        }
    }
    void DelayUnload()
    {
        var package = YooAssets.TryGetPackage(GameEnter.Resource.assetPackageName);

        package.UnloadUnusedAssets();

        package = YooAssets.TryGetPackage(GameEnter.Resource.rawfilePackageName);

        package.UnloadUnusedAssets();

    }


}
