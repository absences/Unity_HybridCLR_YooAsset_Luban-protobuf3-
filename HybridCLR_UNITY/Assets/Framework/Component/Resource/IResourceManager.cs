using Cysharp.Threading.Tasks;
using GameFramework.Resource;

using UnityEngine;
using YooAsset;

public interface IResourceManager
{

    /// <summary>
    /// 初始化接口。
    /// </summary>
    void Initialize();

    /// <summary>
    /// 初始化操作。
    /// </summary>
    /// <returns></returns>
    UniTask<bool> InitPackages();

    /// <summary>
    /// 获取或设置运行模式。
    /// </summary>
    EPlayMode PlayMode
    {
        get;
        set;
    }

    /// <summary>
    /// 获取或设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）。
    /// </summary>
    long Milliseconds
    {
        get;
        set;
    }

    Transform InstanceRoot
    {
        get;
        set;
    }

    /// <summary>
    /// 获取包裹沙盒目录
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    public string GetPackageSandboxPackageRootDirectory(string packageName);


    void Update(float elapseSeconds, float realElapseSeconds);


    void Shutdown();


    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    /// <param name="userData">用户自定义数据。</param>
    void LoadAssetAsync(AssetInfo assetInfo, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData);

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    AssetHandle LoadAssetAsync(AssetInfo assetInfo, uint priority);

    /// <summary>
    /// 异步加载原生资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="loadAssetCallbacks"></param>
    /// <param name="userData"></param>
    void LoadRawFileAsync(string assetName, uint priority, LoadAssetCallbacks loadAssetCallbacks, object userData);

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    RawFileHandle LoadRawFileAsync(string assetName, uint priority);
    /// <summary>
    /// 同步加载
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    RawFileHandle LoadRawFileSync(string assetName);

    /// <summary>
    /// 加载所有资源对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="packageName"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public AllAssetsHandle LoadAllAssetsAsync<T>(string location, uint priority) where T : Object;

    /// <summary>
    /// 加载子资源对象
    /// </summary>
    /// <param name="assetInfo"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public SubAssetsHandle LoadSubAssetsAsync(AssetInfo assetInfo, uint priority);
    /// <summary>
    /// 同步加载资源对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    public T LoadAssetSync<T>(string location) where T : Object;


    /// <summary>
    /// 预下载版本
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public UpdatePrePackageVersionOperation UpdatePrePackageVersionAsync(string packageName);
    /// <summary>
    /// 预下载
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public PreDownloadContentOperation PreDownloadContentAsync(string packageName, string version);
}
