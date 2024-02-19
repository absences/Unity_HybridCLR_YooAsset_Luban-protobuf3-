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
    UniTask InitPackages();

    /// <summary>
    /// 获取或设置运行模式。
    /// </summary>
    EPlayMode PlayMode
    {
        get;
        set;
    }

    /// <summary>
    /// 获取或设置下载文件校验等级。
    /// </summary>
    EVerifyLevel VerifyLevel
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

    void Shutdown();
    /// <summary>
    /// 检查资源是否存在。
    /// </summary>
    /// <param name="assetName">要检查资源的名称。</param>
    /// <returns>检查资源是否存在的结果。</returns>
    bool HasAsset(string assetName);

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    /// <param name="userData">用户自定义数据。</param>
    void LoadAssetAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData);

    /// <summary>
    /// 异步加载原生资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="loadAssetCallbacks"></param>
    /// <param name="userData"></param>
    void LoadRawFileAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData);


    RawFileHandle LoadRawFileAsync(string assetName);
}
