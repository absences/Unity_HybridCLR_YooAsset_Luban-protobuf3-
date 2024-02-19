
using Google.Protobuf;
using System;
using YooAsset;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ConfigManager : IConfigManager
{


    Dictionary<Type, Dictionary<int, IMessage>> allTypesCfg;
    Dictionary<Type, IMessage> instanceCfg;//全局配置

    public bool Loaded => throw new NotImplementedException();

    public ConfigManager()
    {
        allTypesCfg = new Dictionary<Type, Dictionary<int, IMessage>>();
        instanceCfg = new Dictionary<Type, IMessage>();
    }

    public async UniTask LoadAsset<T1, T2>(ConfigParserData<T1, T2> parser) where T1 : class, IMessage where T2 : class, IMessage
    {
        using var assetHandle = GameEnter.Resource.LoadRawFileAsync(parser.fileName);
        await assetHandle;
        var list = parser.getDataList(parser.parseFrom(assetHandle.GetRawFileData()));

        if (parser.getIdFunc == null)
        {
            instanceCfg.Add(typeof(T2), list[0]);
        }
        else
        {
            var cfgDic = new Dictionary<int, IMessage>(list.Count);
            foreach (var item in list)
            {
                var id = parser.getIdFunc(item);
                cfgDic[id] = item;
            }
            allTypesCfg.Add(typeof(T2), cfgDic);
        }
    }

    public T Get<T>(int? id = null) where T : class, IMessage//引用类型约束\接口约束
    {
        var type = typeof(T);

        if (id != null)
        {
            if (!allTypesCfg.TryGetValue(type, out var cfgDic))
            {
                Log.Warning(type.Name + "配置未导入");
                return null;
            }
            if (!cfgDic.TryGetValue((int)id, out IMessage cfgObj))
            {
                Log.Warning("未找到" + type.Name + "的配置   id 为" + id);
                return null;
            }
            return cfgObj as T;
        }
        else
        {
            if (!instanceCfg.TryGetValue(type, out IMessage cfgObj))
            {
                Log.Warning("未找到" + type.Name + "的配置  ");
                return null;
            }
            return cfgObj as T;
        }
    }

    /// <summary>
    /// 获取 T 类型的所有配置
    /// </summary>
    public Dictionary<int, IMessage> GetAll<T>() where T : class, IMessage
    {
        var type = typeof(T);
        if (allTypesCfg.TryGetValue(type, out var dic))
        {
            return dic;
        }
        Log.Warning(type.Name + "没有配置");
        return null;
    }
}
