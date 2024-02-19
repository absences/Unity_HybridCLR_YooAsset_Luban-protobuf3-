using System.IO;
using YooAsset;

/// <summary>
/// 希望将所有热更资源压缩到一个ZIP包里。玩家第一次启动游戏去下载ZIP包，下载完成后解压到沙盒目录下。
/// </summary>
public sealed class DefaultDeliveryQueryServices : IDeliveryQueryServices
{
    public string GetFilePath(string packageName, string fileName)
    {
        throw new System.NotImplementedException();
    }

    public bool Query(string packageName, string fileName, string fileCRC)
    {
        return false;
    }


    //public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
    //{
    //    var resource = GameEnter.Resource;
    //    var path = resource.PackageVersion + "_deliveryFiles" + fileName;
    //    var sandBoxDeliveryFile = Path.Combine(resource.GetPackageSandboxRootDirectory(packageName), path);
    //    return new DeliveryFileInfo()
    //    {
    //        DeliveryFilePath = sandBoxDeliveryFile
    //    };
    //}



    //public bool QueryDeliveryFiles(string packageName, string fileName)
    //{
        
    //    var resource = GameEnter.Resource;
    //    var path = resource.PackageVersion + "_deliveryFiles/" + fileName;
    //    var sandBoxDeliveryFile = Path.Combine(resource.GetPackageSandboxRootDirectory(packageName), path);
    //    var exists= File.Exists(sandBoxDeliveryFile);
    //    Log.Info("query Delivery: " + fileName +"  exists: "+exists );

    //    return exists;
    //}
}
