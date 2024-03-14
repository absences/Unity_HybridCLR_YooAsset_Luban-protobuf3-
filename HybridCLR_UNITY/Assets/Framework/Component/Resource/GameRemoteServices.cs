using YooAsset;
/// <summary>
/// 获取远端资源服务
/// </summary>
public class GameRemoteServices : IRemoteServices
{
    string VersionType;
    int CodeVersion;

    private ResourceComponent _resource;
    private ResourceComponent Resource
    {
        get
        {
            if (_resource == null)
                _resource = GameEnter.GetComponent<ResourceComponent>();
            return _resource;
        }
    }

    public GameRemoteServices()
    {
        VersionType = Resource.VersionType;
        CodeVersion = Resource.AppVersion;
    }

    public string GetRemoteFallbackURL(string packageName, string fileName)
    {
        return string.Format("{0}/{1}/{2}_{3}/{4}/{5}",
         Resource.BackUpResourceSourceUrl, GameUtil.GetPlatformName(), VersionType, CodeVersion, packageName, fileName);
    }

    public string GetRemoteMainURL(string packageName, string fileName)
    {
        return string.Format("{0}/{1}/{2}_{3}/{4}/{5}",
          Resource.ResourceSourceUrl, GameUtil.GetPlatformName(), VersionType, CodeVersion, packageName, fileName);
    }

}
