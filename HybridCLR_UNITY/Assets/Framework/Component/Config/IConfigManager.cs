using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System.Collections.Generic;

public interface IConfigManager 
{
    UniTask LoadAsset<T1, T2>(ConfigParserData<T1, T2> parser) where T1 : class, IMessage where T2 : class, IMessage;

    public T Get<T>(int? id = null) where T : class, IMessage;

    /// <summary>
    /// 获取 T 类型的所有配置
    /// </summary>
    public Dictionary<int, IMessage> GetAll<T>() where T : class, IMessage;
}
