using System.Collections;
using System.Collections.Generic;
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

    public const string rawfilePackage = "rawfilePackage";

    public const string assetPackage = "assetPackage";
    /// <summary>
    /// 资源生命周期服务器。
    /// </summary>
    public ResourceHelper ResourceHelper { get; private set; }

    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    public CancellationToken CancellationToken { get; private set; }

    public bool HasAsset(string assetName)
    {
        return YooAssets.GetAssetInfo(assetName) != null;
    }

    public void Initialize()
    {
        // 初始化资源系统
        YooAssets.Initialize(new YooAssetsLogger());
        YooAssets.SetOperationSystemMaxTimeSlice(Milliseconds);

        ResourceHelper = InstanceRoot.gameObject.AddComponent<ResourceHelper>();
        CancellationToken = ResourceHelper.GetCancellationTokenOnDestroy();
    }
    InitializationOperation InitPackage(string buildPipelineName,  ResourcePackage package)
    {
        switch (PlayMode)
        {
            case EPlayMode.EditorSimulateMode:  // 编辑器下的模拟模式
                {
                    var createParameters = new EditorSimulateModeParameters
                    {
                        SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipelineName, package.PackageName)

                    };
                    return package.InitializeAsync(createParameters);
                }
            case EPlayMode.OfflinePlayMode:
                {
                    var createParameters = new OfflinePlayModeParameters
                    {
                        DecryptionServices = new GameDecryptionServices()//文件解密服务
                    };
                    return package.InitializeAsync(createParameters);
                }
            case EPlayMode.HostPlayMode:
                {
                    var createParameters = new HostPlayModeParameters
                    {
                        DecryptionServices = new GameDecryptionServices(),//文件解密服务
                        DeliveryQueryServices = new DefaultDeliveryQueryServices(),//分发资源服务
                        BuildinQueryServices = new GameQueryServices(),
                        RemoteServices = new GameRemoteServices(),
                        BuildinRootDirectory = "",//内置文件的根路径
                        SandboxRootDirectory = ""//沙盒文件的根路径
                    };
                    return package.InitializeAsync(createParameters);
                }

            case EPlayMode.WebPlayMode:
                {
                    //WebGL平台的专属模式，包括微信小游戏，抖音小游戏都需要选择该模式。
                    var createParameters = new WebPlayModeParameters()
                    {
                        BuildinQueryServices = new GameQueryServices(),
                        RemoteServices = new GameRemoteServices()
                    };
                    return package.InitializeAsync(createParameters);
                }
        }
        return null;
    }
    public async UniTask InitPackages()
    {
        var package = YooAssets.CreatePackage(assetPackage);
        await InitPackage(EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), package);
        YooAssets.SetDefaultPackage(package);

        package = YooAssets.CreatePackage(rawfilePackage);
        await InitPackage(EDefaultBuildPipeline.RawFileBuildPipeline.ToString(), package);
    }

    public void LoadAssetAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
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

        AssetInfo assetInfo = YooAssets.GetAssetInfo(assetName);

        if (assetInfo == null)
        {
            string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetName);
            if (loadAssetCallbacks.LoadAssetFailureCallback != null)
            {
                loadAssetCallbacks.LoadAssetFailureCallback(assetName, EOperationStatus.Failed, errorMessage, userData);
                return;
            }

            throw new GameFrameworkException(errorMessage);
        }
        UniTask.Create(async () =>
        {
            AssetHandle handle = YooAssets.LoadAssetAsync(assetInfo);

            await handle.ToUniTask(ResourceHelper);

            //AssetOperationHandle handle = (AssetOperationHandle)handleBase;
            if (handle == null || handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
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
        });

    }

    public void LoadRawFileAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
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

        var assetPackage = YooAssets.TryGetPackage(rawfilePackage);

        AssetInfo assetInfo = assetPackage.GetAssetInfo(assetName);

        if (assetInfo == null)
        {
            string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetName);
            if (loadAssetCallbacks.LoadAssetFailureCallback != null)
            {
                loadAssetCallbacks.LoadAssetFailureCallback(assetName, EOperationStatus.Failed, errorMessage, userData);
                return;
            }

            throw new GameFrameworkException(errorMessage);
        }
        UniTask.Create(async () =>
        {
            RawFileHandle handle = assetPackage.LoadRawFileAsync(assetInfo);

            await handle.ToUniTask(ResourceHelper);

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

        });
    }
    /// <summary>
    /// 异步加载原生文件
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public RawFileHandle LoadRawFileAsync(string assetName)
    {
        var assetPackage = YooAssets.TryGetPackage(rawfilePackage);

        AssetInfo assetInfo = assetPackage.GetAssetInfo(assetName);

        return assetPackage.LoadRawFileAsync(assetInfo);
    }
    public string GetPackageSandboxPackageRootDirectory(string packageName)
    {
        ResourcePackage package = YooAssets.GetPackage(packageName);

        return package.GetPackageSandboxPackageRootDirectory();
    }
   
    public void Shutdown()
    {
        YooAssets.Destroy();
    }
}
