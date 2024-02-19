using Cfg;
using Cysharp.Threading.Tasks;
using GameFramework;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigParserData<T1, T2> where T1 : IMessage where T2 : IMessage
{
    public Func<byte[], T1> parseFrom;
    public Func<T1, RepeatedField<T2>> getDataList;
    public string fileName;
    public Func<T2, int> getIdFunc;
}

public class ConfigComponent : BaseGameComponent
{

    private IConfigManager m_ConfigManager = null;


    public async UniTask Init()
    {
        m_ConfigManager = new ConfigManager();


        await (
              m_ConfigManager.LoadAsset(AssetsPathCfgs.parserData)
            , m_ConfigManager.LoadAsset(EntityCfgs.parserData)
            , m_ConfigManager.LoadAsset(SoundCfgs.parserData)
         );

        Log.Info("配置加载完毕");
    }

    public T Get<T>(int? id = null) where T : class, IMessage//引用类型约束\接口约束
    {
        return m_ConfigManager.Get<T>(id);
    }

    /// <summary>
    /// 获取 T 类型的所有配置
    /// </summary>
    public Dictionary<int, IMessage> GetAll<T>() where T : class, IMessage
    {
        return m_ConfigManager.GetAll<T>();
    }
}
